using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.HotelModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HulubejeBooking.Controllers.HotelController
{
    public class GuestInfoController : Controller
    {
        private IHttpContextAccessor? _httpContextAccessor;
        private readonly AuthenticationManager _authenticationManager;

        public GuestInfoController(IHttpContextAccessor? httpContextAccessor, AuthenticationManager authenticationManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
        }
        public async Task<IActionResult> Index(double averageAmount, double totalPrice, int roomTypeCode, string roomTypeDescription, string roomPakages, string ratePolicy, int rateCode, int rateCodeDetail)
        {
            var roomDetails = new RoomType
			{
			   AverageAmount = averageAmount,
               TotalAmount = totalPrice,
               RoomTypeCode = roomTypeCode,
               RoomTypeDescription = roomTypeDescription,
               RatePolicy = ratePolicy,
			   Packagelist = roomPakages,
               RateCodeDetail = rateCodeDetail,
               RateCode = rateCode,
			};
            var roomDetailsJson = JsonConvert.SerializeObject(roomDetails);
            HttpContext.Session.SetString("RoomType", roomDetailsJson);
            var RoomFormData = HttpContext.Session.GetString("RoomFormData");
            var RoomFormDatajson = new RoomFormData(); 

			if (RoomFormData != null)
            {
				RoomFormDatajson = JsonConvert.DeserializeObject<RoomFormData>(RoomFormData);
			}
            var identificationResult = await _authenticationManager.identificationValid();
            if (identificationResult != null)
            {
                ViewBag.isValid = identificationResult.isValid;
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
                ViewBag.Gender = identificationResult?.UserData.Gender;
            }

            return View(RoomFormDatajson);
        }
    }
}
