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
using HulubejeBooking.Models.Authentication;

namespace HulubejeBooking.Controllers.Payment
{
    public class Spayment : Controller
    {
        private readonly ILogger<Spayment> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;

        public Spayment(ILogger<Spayment> logger, IHttpClientFactory httpClientFactory, IHttpContextAccessor? httpContextAccessor)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            var paymentOptionsJson = HttpContext.Session.GetString("PaymentOptions");
            var value = HttpContext.Session.GetString("cinema");
            HttpContext.Session.Remove("cinema");
            if  (value != null)
            {
                ViewBag.CoutDown = value;
            }

            var userDataCookie = _httpContextAccessor?.HttpContext?.Request.Cookies[CNET_WebConstants.IdentificationCookie];
            if (!string.IsNullOrEmpty(userDataCookie))
            {
                var user = JsonConvert.DeserializeObject<UserInformation>(userDataCookie);
                ViewBag.FirstName = user?.firstName;
                ViewBag.LastName = user?.lastName;
                ViewBag.MiddleName = user?.middleName;
                ViewBag.Personalattachment = user?.personalattachment;
                ViewBag.SuccessCode = user?.successCode;
                ViewBag.Idnumber = user?.idnumber;
                ViewBag.Idtype = user?.idtype;
                ViewBag.Dob = user?.dob;
                ViewBag.Idattachment = user?.idattachment;
                ViewBag.PhoneNumber = user?.phoneNumber;
                ViewBag.EmailAddress = user?.emailAddress;
            }
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
