using Microsoft.AspNetCore.Mvc;     
using HulubejeBooking.Models.PaymentModels;
using HulubejeBooking.Models;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Net.Http.Headers;
using HulubejeBooking.Models.PaymentModels.HotlePaymentModels;
using Microsoft.AspNetCore.Http;

namespace HulubejeBooking.Controllers.Payment
{
    public class Spayment : Controller
    {
        private readonly ILogger<Spayment> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public Spayment(ILogger<Spayment> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> PaymentAsync([FromBody] TransactionModel data) 
        {
            var _client = _httpClientFactory.CreateClient("Payment");
            var param = HttpContext.Session.GetString("transactionDatas");

            var paymentValidation = new PaymentValidation();

            if (!HttpContext.Session.TryGetValue("AccessToken", out var accessTokenBytes))
            {
                return Error();
            }

            string accessToken = Encoding.UTF8.GetString(accessTokenBytes);

            if (!string.IsNullOrEmpty(param)) 
            {
               var newParam = JsonConvert.DeserializeObject<TransactionModel>(param);

                var request = new  
                { 
                    newParam?.UserMobileNumber,
                    newParam?.SupplierTin,
                    newParam?.SupplierOUD,
                    newParam?.TransactionId,
                    newParam?.Amount,
                    newParam?.PaymentProviderOUD,
                    data.Pin,
                    AdditionalParameters = new
                    {
                        newParam?.AdditionalParameters?.ReferenceNumber
                    }

                };
                
                HttpContext.Session.Remove("transactionDatas");

                var reqJson = JsonConvert.SerializeObject(request);
                var content = new StringContent(reqJson, Encoding.UTF8, "application/json");
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "payment/transaction", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();

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
                            data.Pin,
                            ExpirationDate = ""

                        },
                        IsAsyncMode = false

                    };
                    var isSuccessfulJson = JsonConvert.SerializeObject(isSuccessful);
                    HttpContext.Session.SetString("PaymentInfo", isSuccessfulJson);

                     paymentValidation = JsonConvert.DeserializeObject<PaymentValidation>(responseData);

                    
                    var ValidationDataJson = JsonConvert.SerializeObject(paymentValidation);

                    HttpContext.Session.SetString("ValidationInfo", ValidationDataJson);

                    return RedirectToAction("PaymentCommon", "SHotelpayment");
                }

         

                
                

                //if (response.IsSuccessStatusCode) 
                //{
                //    string responseData = await response.Content.ReadAsStringAsync();
                //    PaymentValidation validationData = JsonConvert.DeserializeObject<PaymentValidation>(responseData);

                //}
                //else
                //{
                //    string responseData = await response.Content.ReadAsStringAsync();
                //    PaymentValidation validationData = JsonConvert.DeserializeObject<PaymentValidation>(responseData);


                //}

            }
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
