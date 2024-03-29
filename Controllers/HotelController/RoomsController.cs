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
            var identificationResult = await _authenticationManager.identificationValid();
            if (identificationResult != null)
            {
                ViewBag.isVaild = identificationResult.isValid;
                ViewBag.isLoggedIn = identificationResult.isLoggedIn;
                ViewBag.FirstName = identificationResult?.UserData.FirstName;
                ViewBag.LastName = identificationResult?.UserData.LastName;
                ViewBag.MiddleName = identificationResult?.UserData.MiddleName;
                ViewBag.Personalattachment = identificationResult?.UserData.PersonalAttachment;
                ViewBag.Idnumber = identificationResult?.UserData.IdNumber;
                ViewBag.Idtype = identificationResult?.UserData.IdType;
                ViewBag.Dob = identificationResult?.UserData.Dob;
                ViewBag.Idattachment = identificationResult?.UserData.IdAttachment;
                ViewBag.PhoneNumber = identificationResult?.UserData.Code;
                ViewBag.EmailAddress = identificationResult?.UserData.Email;
            }
            var loginInfo = HttpContext.Session.GetString("IsLogin");
            var login = "";
            if (loginInfo != null)
            {
                login = JsonConvert.DeserializeObject<string>(loginInfo);
            }
            HttpContext.Session.Remove("IsLogin");
            if (login != "Yes")
            {
                if (identificationResult!=null && !identificationResult.isValid && !identificationResult.isLoggedIn)
                {
                    string validation = "Hotel";
                    var validationJson = JsonConvert.SerializeObject(validation);
                    HttpContext.Session.SetString("SignInInformation", validationJson);
                    TempData["ErrorMessage"] = "Please login to proceed further.";
                    return RedirectToAction("Index", "Signin");
                }
            }

            var viewModelJson = HttpContext.Session.GetString("AvailabilityViewModel");
            var viewModel = new GetRooms();
            if (!string.IsNullOrEmpty(viewModelJson))
            {
                //HttpContext.Session.Remove("AvailabilityViewModel");
                viewModel = JsonConvert.DeserializeObject<GetRooms>(viewModelJson);
            }
            return View(viewModel);
        }

    }
    
}
