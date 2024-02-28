using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HulubejeBooking.Models.PaymentModels;
using HulubejeBooking.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Controllers.Authentication;

namespace HulubejeBooking.Controllers.Payment
{
    public class ApaymentController : Controller
    {
        private readonly ILogger<ApaymentController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationManager _authenticationManager;

        public ApaymentController(ILogger<ApaymentController> logger, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, AuthenticationManager authenticationManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _authenticationManager = authenticationManager;
        }

        public async Task<IActionResult> IndexAsync()
        {
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
            var b = await _authenticationManager.identificationValid();
            ViewBag.isVaild = b.isValid;
            ViewBag.isLoggedIn = b.isLoggedIn;
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

                    HttpContext.Session.SetString("AsyncPaymentInfo", ValidationDataJson);
                     
                                                    
                }
                await Task.Delay(5000);
                if (pay!=null && pay.IsFulfilled && pay.IsResolved)
                {
                    return RedirectToAction("PaymentCommon", "SHotelpayment");

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
