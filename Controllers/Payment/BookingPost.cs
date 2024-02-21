using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.PaymentModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HulubejeBooking.Controllers.Payment
{
    public class BookingPost : Controller
    {
        private readonly AuthenticationManager _authenticationManager;
        private IHttpContextAccessor? _httpContextAccessor;
        public BookingPost(IHttpContextAccessor httpContextAccessor, AuthenticationManager authenticationManager)
        {
            _httpContextAccessor = httpContextAccessor;
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
            var paymentDoneModelJson = HttpContext.Session.GetString("PaymentDoneModel");
            //HttpContext.Session.Remove("PaymentDoneModel");
            var paymentDoneModel = !string.IsNullOrWhiteSpace(paymentDoneModelJson) ? JsonConvert.DeserializeObject<PaymentValidation>(paymentDoneModelJson) : null;
            HttpContext.Session.Remove("cinema");
            return View(paymentDoneModel);
        }
    }
}
