using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.PaymentModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace HulubejeBooking.Controllers.Payment
{
    public class PaymentOptionsController : Controller
    {
        private IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;
        private AuthenticationManager _authenticationManager;

        public PaymentOptionsController(IHttpClientFactory httpClientFactory, IHttpContextAccessor? httpContextAccessor, AuthenticationManager authenticationManager)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;

        }
        public async Task<IActionResult> Index()
        {
            var userDataCookie = _httpContextAccessor?.HttpContext?.Request.Cookies[CNET_WebConstants.IdentificationCookie];
            string? code = null;
            if (!string.IsNullOrEmpty(userDataCookie))
            {
                var user = JsonConvert.DeserializeObject<UserInformation>(userDataCookie);
                code = user?.phoneNumber;
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
            var paymentOptionsData = HttpContext.Session.GetString("paymentOptrions");
            var paymentOptions = paymentOptionsData != null ? JsonConvert.DeserializeObject<PaymentProcessorResponse>(paymentOptionsData) : new PaymentProcessorResponse();

            return View(paymentOptions);
        }
    }
}
