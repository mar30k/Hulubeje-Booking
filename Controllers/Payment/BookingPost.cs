using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.PaymentModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HulubejeBooking.Controllers.Payment
{
    public class BookingPost : Controller
    {
        private IHttpContextAccessor? _httpContextAccessor;
        public BookingPost(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            var userDataCookie = _httpContextAccessor?.HttpContext?.Request.Cookies[CNET_WebConstants.IdentificationCookie];
            if (!string.IsNullOrEmpty(userDataCookie))
            {
                var user = JsonConvert.DeserializeObject<UserInformation>(userDataCookie);
                ViewBag.UserName = user?.firstName;
                ViewBag.LastName = user?.lastName;
                ViewBag.MiddleName = user?.middleName;
                ViewBag.Image = user?.personalattachment;
                ViewBag.Email = user?.emailAddress;
            }
            var paymentDoneModelJson = HttpContext.Session.GetString("PaymentDoneModel");
            //HttpContext.Session.Remove("PaymentDoneModel");
            var paymentDoneModel = !string.IsNullOrWhiteSpace(paymentDoneModelJson) ? JsonConvert.DeserializeObject<PaymentValidation>(paymentDoneModelJson) : null;

            return View(paymentDoneModel);
        }
    }
}
