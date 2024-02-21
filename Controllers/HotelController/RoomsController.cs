using HulubejeBooking.Models.HotelModels;
using HulubejeBooking.Controllers.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HulubejeBooking.Models.Authentication;

namespace HulubejeBooking.Controllers.HotelController
{
    public class RoomsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HotelListBuffer _hotelListBuffer;
        private readonly AuthenticationManager _authenticationManager;

        public RoomsController(
            IHttpClientFactory httpClientFactory,
            AuthenticationManager authenticationManager,
            HotelListBuffer hotelListBuffer, IHttpContextAccessor httpContextAccessor)
        {
            _authenticationManager = authenticationManager;
            _httpClientFactory = httpClientFactory;
            _hotelListBuffer = hotelListBuffer;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
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
			var identificationResult = await _authenticationManager.identificationValid();
            ViewBag.isVaild = identificationResult.isValid;
            ViewBag.isLoggedIn = identificationResult.isLoggedIn;
            var loginInfo = HttpContext.Session.GetString("IsLogin");
            var login = "";
            if (loginInfo != null)
            {
                login = JsonConvert.DeserializeObject<string>(loginInfo);
            }
            HttpContext.Session.Remove("IsLogin");
            if (login != "Yes")
            {
                if (!identificationResult.isValid && !identificationResult.isLoggedIn)
                {
                    string validation = "Hotel";
                    var validationJson = JsonConvert.SerializeObject(validation);
                    HttpContext.Session.SetString("SignInInformation", validationJson);
                    TempData["ErrorMessage"] = "Please login to proceed further.";
                    return RedirectToAction("Index", "Signin");
                }
            }

            var viewModelJson = HttpContext.Session.GetString("AvailabilityViewModel");
            var viewModel = new AvailabilityViewModel();
            if (!string.IsNullOrEmpty(viewModelJson))
            {
                //HttpContext.Session.Remove("AvailabilityViewModel");
                viewModel = JsonConvert.DeserializeObject<AvailabilityViewModel>(viewModelJson);
            }
            return View(viewModel);
        }

    }
    
}
