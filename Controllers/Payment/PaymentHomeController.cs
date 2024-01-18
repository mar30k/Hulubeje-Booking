using HulubejeBooking.Models;
using HulubejeBooking.Models.HotelModels;
using HulubejeBooking.Models.PaymentModels;
using HulubejeBooking.Models.PaymentModels.HotlePaymentModels;

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
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
            var validation = new RequestWrapper
            {
                GuestInfoData = data.GuestInfoData,
                CinemaDetailsData = data.CinemaDetailsData
            };
            var validationJson = JsonConvert.SerializeObject(validation);
            HttpContext.Session.SetString("Data", validationJson);
            try
            {
                string password = pass;
                var UserMobileNumber = data.AuthorizePaymentData.UserMobileNumber;
                var SupplierTin = data.AuthorizePaymentData.SupplierTin;
                var SupplierOUD = data.AuthorizePaymentData.SupplierOUD;

                if (data.CinemaDetailsData is not null)
                {
                    var val = "this is cinema";
                    HttpContext.Session.SetString("cinema", val);
                }


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
                        var accessToken = JsonConvert.DeserializeObject<AuthorizationAccessTokenModel>(authResponseData);

                        var token = accessToken?.accessToken;
                        if (token != null)
                        {
                            HttpContext.Session.SetString("AccessToken", token);
                        }

                        // GetPaymentOptions request
                        if (HttpContext.Session.TryGetValue("AccessToken", out var accessTokenBytes))
                        {
                            string accessTokenValue = Encoding.UTF8.GetString(accessTokenBytes);
                            var parameters = new
                            {
                                UserMobileNumber,
                                SupplierTin,
                                SupplierOUD,
                                data.AuthorizePaymentData.Amount,
                            };
                            var paymentInfoJson = JsonConvert.SerializeObject(parameters);

                            HttpContext.Session.SetString("paymentInfo", paymentInfoJson);


                            var paymentOptions = new List<PaymentOptionModel>();

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
                                var transactionId = JsonConvert.DeserializeObject<string>(codeResponseData);

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
        [HttpPost]
        public async Task<IActionResult> SelectedOptionAsync([FromBody] RequestWrapper data)
        {
            try
            {
                var operationMode = data.FlagData.operationMode;
                var value = OperationModeController.FlagChecker(operationMode);
                var synch = value.Synchronous;
                var asynch = value.Asynchronous;
                var Currency = "etb";

                var vCode = HttpContext.Session.GetString("VoucherCode");
                var TransactionId = !string.IsNullOrWhiteSpace(vCode)
                    ? JsonConvert.DeserializeObject<string>(vCode)
                    : null;

                var parameters = HttpContext.Session.GetString("paymentInfo");
                var newParam = !string.IsNullOrWhiteSpace(parameters)
                    ? JsonConvert.DeserializeObject<AuthorizePayment>(parameters)
                    : null;



                if (!HttpContext.Session.TryGetValue("AccessToken", out var accessTokenBytes))
                {
                    return Error();
                }

                string accessToken = Encoding.UTF8.GetString(accessTokenBytes);

                using (var _client = _httpClientFactory.CreateClient("Payment"))
                {
                    if (synch)
                    {
                        var param = new
                        {
                            newParam?.UserMobileNumber,
                            newParam?.SupplierTin,
                            newParam?.SupplierOUD,
                            TransactionId,
                            data.AuthorizePaymentData.PaymentProviderOUD,
                            newParam?.Amount,
                            AdditionalParameters = new
                            {
                                data.AuthorizePaymentData.AdditionalParameters?.ReferenceNumber
                            }
                        };


                        var jsonRequest = JsonConvert.SerializeObject(param);

                        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "payment/authorization", content);

                        if (response.IsSuccessStatusCode)
                        {
                           


                            var transactionModel = new
                            {
                                newParam?.UserMobileNumber,
                                newParam?.SupplierTin,
                                newParam?.SupplierOUD,
                                TransactionId,
                                data.AuthorizePaymentData.PaymentProviderOUD,
                                newParam?.Amount,
                                AdditionalParameters = new
                                {
                                    data.AuthorizePaymentData.AdditionalParameters?.ReferenceNumber
                                }
                            };
                            var values = JsonConvert.SerializeObject(transactionModel);

                            HttpContext.Session.SetString("transactionDatas", values);

                            return Json(new { cardText = "telebirr SMS OTP" });

                        }
                    }
                    if (asynch)
                    {
                        var param = new
                        {
                            newParam?.UserMobileNumber,
                            newParam?.SupplierTin,
                            newParam?.SupplierOUD,
                            TransactionId,
                            data.TransactionData.PaymentProviderOUD,
                            newParam?.Amount,
                            data.TransactionData.Pin,
                            AdditionalParameters = new
                            {
                                data.TransactionData.AdditionalParameters?.ReferenceNumber
                            }
                        };

                        var jsonRequest = JsonConvert.SerializeObject(param);
                        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                        var transactionVerification = new
                        {
                            newParam?.SupplierOUD,
                            TransactionId,
                            newParam?.Amount
                        };
                        var values = JsonConvert.SerializeObject(param);

                        HttpContext.Session.SetString("paymentValidation", values);

                        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "payment/transaction", content);
                        string responseData = await response.Content.ReadAsStringAsync();

                        //var paymentValidation = JsonConvert.DeserializeObject<PaymentValidation>(responseData);

                        HttpContext.Session.SetString("ValidationInfo", responseData);

                        if (response.IsSuccessStatusCode)
                        {


                            return Json(new { cardText = "telebirr USSD Push" });

                        }
                    }
                    if (!synch && !asynch)
                    {
                        var param = new
                        {
                            newParam.UserMobileNumber,
                            newParam.SupplierTin,
                            newParam.SupplierOUD,
                            TransactionId,
                            data.TransactionData.PaymentProviderOUD,
                            newParam.Amount,
                            data.TransactionData.Pin,
                            Currency

                        };
                        var jsonRequest = JsonConvert.SerializeObject(param);
                        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                        HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "payment/transaction", content);
                        if (response.IsSuccessStatusCode)
                        {
                            var responseData = await response.Content.ReadAsStringAsync();
                            var apiResponse = JsonConvert.DeserializeObject<BoAResponse>(responseData);

                            var cardText = "BOA Card Payment"; 
                            var redirectUrl = apiResponse?.additionalParameters?.RedirectUrl;

                            
                            var paymentResponse = new PaymentResponse
                            {
                                cardText = cardText,
                                RedirectUrl = redirectUrl
                            };

                            return Json(paymentResponse);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }

            return Error();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}
