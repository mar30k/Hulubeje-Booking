using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HulubejeBooking.Models.PaymentModels;
using HulubejeBooking.Models.PaymentModels.HotlePaymentModels;
using HulubejeBooking.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace HulubejeBooking.Controllers.Payment.HotlePaymentModels
{
    public class AHotelpayment : Controller
    {
        private readonly ILogger<AHotelpayment> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public AHotelpayment(ILogger<AHotelpayment> logger, IHttpClientFactory httpClientFactory)
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
                var newParam = JsonConvert.DeserializeObject<TransactionModel>(param);
                var SupplierTin = newParam.SupplierTin;
                var TransactionId = newParam.TransactionId;
                var Amount = newParam.Amount;

                var parameters = new
                {
                    SupplierTin,
                    TransactionId,
                    Amount,
                };
                GuestInfoModel hotelPayment = new GuestInfoModel
                {
                    paymentInfo = new PaymentInfoModel
                    {
                        isAsyncMode = true,
                        userAccessToken = accessToken,
                        paymentTransactionRequest = new PaymentTransactionRequestModel
                        {
                            UserMobileNumber = newParam.UserMobileNumber,
                            SupplierTin = newParam.SupplierTin,
                            SupplierOUD = newParam.SupplierOUD,
                            TransactionId = newParam.TransactionId,
                            Amount = newParam.Amount,
                            PaymentProviderOUD = newParam.PaymentProviderOUD,
                            Pin = null,
                            ExpirationDate = "",

                        }
                    },

                };

                string queryString = string.Join("&", parameters.GetType().GetProperties()
                    .Select(prop => $"{prop.Name}={Uri.EscapeDataString(prop.GetValue(parameters)?.ToString())}"));

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                string apiUrl = $"{_client.BaseAddress}payment/transaction?{queryString}";

                bool isSuccess = false;
                DateTime endTime = DateTime.Now.AddMinutes(0.5); 

                while (DateTime.Now < endTime && !isSuccess)
                {
                    HttpResponseMessage response = await _client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        pay = JsonConvert.DeserializeObject<PaymentResponseModel>(responseData);
                        isSuccess = pay.IsFulfilled; 
                    }

                    await Task.Delay(5000); 
                }
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
