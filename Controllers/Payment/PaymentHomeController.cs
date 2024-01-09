using HulubejeBooking.Models;
using HulubejeBooking.Models.PaymentModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace HulubejeBooking.Controllers.Payment
{
    public class PaymentHomeController : Controller
    {
        private string pass = "$2a$10$um.537vTrMuSalYEVoLUbOgCkuvGBmoViI08GBPGXbP8rYUmOtaK6";
        private readonly ILogger<PaymentHomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public PaymentHomeController(ILogger<PaymentHomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> IndexAsync([FromBody] RequestWrapper data)
        {
            try
            {
                string password = pass;
                var UserMobileNumber = data.AuthorizePaymentData.UserMobileNumber;
                var SupplierTin = data.AuthorizePaymentData.SupplierTin;
                var SupplierOUD = data.AuthorizePaymentData.SupplierOUD;

                using (var _client = _httpClientFactory.CreateClient("Payment"))
                {
                    // Authentication request
                    var authRequestData = new { password };
                    var authJsonContent = JsonConvert.SerializeObject(authRequestData);
                    var authContent = new StringContent(authJsonContent, Encoding.UTF8, "application/json");

                    HttpResponseMessage authResponse = await _client.PostAsync(_client.BaseAddress + "payment/authentication", authContent);

                    if (authResponse.IsSuccessStatusCode)
                    {
                        string authResponseData = await authResponse.Content.ReadAsStringAsync();
                        AuthorizationAccessTokenModel accessToken = JsonConvert.DeserializeObject<AuthorizationAccessTokenModel>(authResponseData);

                        TempData["AccessToken"] = accessToken.accessToken;
                        HttpContext.Session.SetString("AccessToken", accessToken.accessToken);

                        // GetPaymentOptions request
                        if (HttpContext.Session.TryGetValue("AccessToken", out var accessTokenBytes))
                        {
                            string accessTokenValue = Encoding.UTF8.GetString(accessTokenBytes);
                            var parameters = new
                            {
                                UserMobileNumber,
                                SupplierTin ,
                                SupplierOUD 
                            };

                            List<PaymentOptionModel> paymentOptions = new List<PaymentOptionModel>();

                            string queryString = string.Join("&", parameters.GetType().GetProperties()
                                .Select(prop => $"{prop.Name}={Uri.EscapeDataString(prop.GetValue(parameters)?.ToString())}"));

                            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenValue);
                            var baseUri = "https://api-hulubeje.cnetcommerce.com/api";

                            string apiUrl = $"{_client.BaseAddress}payment/options?{queryString}";
                            HttpResponseMessage optionsResponse = await _client.GetAsync(apiUrl);

                            string requestUrl = $"{baseUri}/Ecommerce/generateVoucherCode?voucherCodeBase={data.AuthorizePaymentData.UserMobileNumber}";
                            HttpResponseMessage codeResponse = await _client.GetAsync(requestUrl);

                            if (optionsResponse.IsSuccessStatusCode && codeResponse.IsSuccessStatusCode)
                            {
                                string optionsResponseData = await optionsResponse.Content.ReadAsStringAsync();
                                string codeResponseData = await codeResponse.Content.ReadAsStringAsync();
                                string transactionId = JsonConvert.DeserializeObject<string>(codeResponseData);

                                HttpContext.Session.SetString("VoucherCode", JsonConvert.SerializeObject(transactionId));
                                paymentOptions = JsonConvert.DeserializeObject<List<PaymentOptionModel>>(optionsResponseData);

                                var paymentOptionsJson = JsonConvert.SerializeObject(paymentOptions);
                                HttpContext.Session.SetString("PaymentOptions", paymentOptionsJson);

                                 return Json(new PaymentOptionModel());
                            }
                            else
                            {
                                string errorContent = await optionsResponse.Content.ReadAsStringAsync();

                                Console.WriteLine($"Error Content: {errorContent}");
                                Console.WriteLine($"Status Code: {optionsResponse.StatusCode}");
                                Console.WriteLine($"Reason Phrase: {optionsResponse.ReasonPhrase}");

                                return Error();
                            }
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        return Error();
                    }
                }
            }
            catch (Exception ex)
            {
                return Error();
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}
