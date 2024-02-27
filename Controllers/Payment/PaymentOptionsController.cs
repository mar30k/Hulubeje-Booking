using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HulubejeBooking.Models.PaymentModels;
using HulubejeBooking.Controllers;
using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Controllers.Authentication;

namespace Payment.Controllers
{
    public class PaymentOptionsController : Controller
    {
        private readonly AuthenticationManager _authenticationManager;
        private IHttpContextAccessor? _httpContextAccessor;
        public PaymentOptionsController(IHttpContextAccessor httpContextAccessor ,AuthenticationManager authenticationManager)
        {
            _httpContextAccessor = httpContextAccessor; 
            _authenticationManager = authenticationManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> PaymentOption()
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
            var paymentOptionsJson = HttpContext.Session.GetString("PaymentOptions");
            var value = HttpContext.Session.GetString("cinema");
            ViewBag.CoutDown= value;
            if (!string.IsNullOrEmpty(paymentOptionsJson))
            {
                var viewPayments = JsonConvert.DeserializeObject<List<PaymentOptionModel>>(paymentOptionsJson);
                var wrapper = new Wrapper
                {
                    PaymentOptions = viewPayments,  
                    Boa = new BoAModel()  
                };
                return View(wrapper);
            }
            else
            {
                return View(new Wrapper());
            }
        }



    }
}
