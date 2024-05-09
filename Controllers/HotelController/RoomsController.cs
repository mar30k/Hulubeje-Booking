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
        private readonly AuthenticationManager _authenticationManager;

        public RoomsController(
            IHttpClientFactory httpClientFactory,
            AuthenticationManager authenticationManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _authenticationManager = authenticationManager;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var roomForm = HttpContext.Session.GetString("RoomFormData");
            var roomFormData = roomForm!=null ? JsonConvert.DeserializeObject<RoomFormData>(roomForm) : null;
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

            if (!string.IsNullOrEmpty(viewModelJson) && !string.IsNullOrEmpty(roomForm))
            {
                //HttpContext.Session.Remove("AvailabilityViewModel");
                GetRooms? viewModel = JsonConvert.DeserializeObject<GetRooms>(viewModelJson);
                roomFormData = JsonConvert.DeserializeObject<RoomFormData>(roomForm);
                if (roomFormData != null && viewModel!=null)
                {
                    viewModel.RoomFormData = roomFormData;
                }
                return View(viewModel);
			}
            else
            {
				TempData["ErrorMessage"] = "Session Has Expired Please Restart the Booking Process";
				return RedirectToAction("Index", "Home");
			}
        }

    }
    
}
