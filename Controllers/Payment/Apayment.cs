using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HulubejeBooking.Models.PaymentModels;
using HulubejeBooking.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace HulubejeBooking.Controllers.Payment
{
    public class Apayment : Controller
    {
        private readonly ILogger<Apayment> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public Apayment(ILogger<Apayment> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var _client = _httpClientFactory.CreateClient("Payment");
            var param = HttpContext.Session.GetString("paymentValidation");
            var pay = new PaymentResponseModel();

            if (!HttpContext.Session.TryGetValue("AccessToken", out var accessTokenBytes))
            {
                return Error();
            }

            string accessToken = Encoding.UTF8.GetString(accessTokenBytes);

            if (!string.IsNullOrEmpty(param))
            {
                var paymentValidation  = new PaymentValidation();
                var newParam = JsonConvert.DeserializeObject<TransactionModel>(param);
                var SupplierTin = newParam?.SupplierTin;
                var TransactionId = newParam?.TransactionId;
                var Amount = newParam?.Amount;
                var userMobile = newParam?.UserMobileNumber;

                var parameters = new
                {
                    SupplierTin,
                    TransactionId,
                    Amount,
                };

                string queryString = string.Join("&", parameters.GetType().GetProperties()
                                  .Select(prop =>
                                  {
                                      var value = prop.GetValue(parameters);
                                      if (value != null)
                                      {
                                          return $"{prop.Name}={Uri.EscapeDataString(value.ToString())}";
                                      }
                                      return null;
                                  })
                                  .Where(s => s != null));

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                string apiUrl = $"{_client.BaseAddress}payment/transaction?{queryString}";

                bool isSuccess = false;
                DateTime endTime = DateTime.Now.AddMinutes(0.5); 

                while (DateTime.Now < endTime && !isSuccess)
                {
                    HttpResponseMessage response = await _client.GetAsync(apiUrl);
                    var responseData = await response.Content.ReadAsStringAsync();
                    
                    pay = JsonConvert.DeserializeObject<PaymentResponseModel>(responseData);
                    if (pay != null )
                    {
                        isSuccess = pay.IsFulfilled;
                    }
                    var isSuccessful = new
                    {
                        accessToken,
                        paymentTransactionRequest = new
                        {
                            newParam?.UserMobileNumber,
                            newParam?.SupplierTin,
                            newParam?.SupplierOUD,
                            newParam?.TransactionId,
                            newParam?.Amount,
                            newParam?.PaymentProviderOUD,
                            newParam?.Pin,
                            ExpirationDate = ""

                        },
                        IsAsyncMode = false

                    };
                    var isSuccessfulJson = JsonConvert.SerializeObject(isSuccessful);
                    HttpContext.Session.SetString("PaymentInfo", isSuccessfulJson);

                    var ValidationDataJson = JsonConvert.SerializeObject(pay);

                    HttpContext.Session.SetString("mark", ValidationDataJson);
                     
                                                    
                }
                await Task.Delay(5000);
                return RedirectToAction("PaymentCommon", "SHotelpayment");
            }

            return View(pay);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
