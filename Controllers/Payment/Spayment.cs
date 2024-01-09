using Microsoft.AspNetCore.Mvc;
using HulubejeBooking.Models.PaymentModels;
using HulubejeBooking.Models;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Net.Http.Headers;

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
                    newParam.UserMobileNumber,
                    newParam.SupplierTin,
                    newParam.SupplierOUD,
                    newParam.TransactionId,
                    newParam.Amount,
                    newParam.PaymentProviderOUD,
                    data.Pin,
                    AdditionalParameters = new
                    {
                        newParam.AdditionalParameters?.ReferenceNumber
                    }

                };
                var reqJson = JsonConvert.SerializeObject(request);
                var content = new StringContent(reqJson, Encoding.UTF8, "application/json");
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "payment/transaction", content);
                if (response.IsSuccessStatusCode) 
                {
                    HttpContext.Session.Remove("transactionDatas");
                }

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
