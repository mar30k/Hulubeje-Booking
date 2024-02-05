using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HulubejeBooking.Models.PaymentModels;
using HulubejeBooking.Controllers;
using HulubejeBooking.Models.Authentication;

namespace Payment.Controllers
{
    public class PaymentOptions : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;
        public PaymentOptions(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor; 
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PaymentOption()
        {
            var userDataCookie = _httpContextAccessor?.HttpContext?.Request.Cookies[CNET_WebConstants.IdentificationCookie];
            if (!string.IsNullOrEmpty(userDataCookie))
            {
                var user = JsonConvert.DeserializeObject<UserInformation>(userDataCookie);
                ViewBag.UserName = user?.firstName;
                ViewBag.LastName = user?.lastName;
                ViewBag.MiddleName = user?.middleName;
                ViewBag.Image = user?.personalattachment;
                ViewBag.SuccessCode = user?.successCode;
                ViewBag.Inumber = user?.idnumber;
                ViewBag.Idtype = user?.idtype;
                ViewBag.Dob = user?.dob;
                ViewBag.Idattachment = user?.idattachment;
                ViewBag.PhoneNumber = user?.phoneNumber;
                ViewBag.EmailAddress = user?.emailAddress;
            }
            var paymentOptionsJson = HttpContext.Session.GetString("PaymentOptions");
            var value = HttpContext.Session.GetString("cinema");
            HttpContext.Session.Remove("cinema");
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
                return View(new PaymentOptionModel());
            }
        }



    }
}
