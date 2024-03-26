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
        public async Task<IActionResult> IndexAsync(double averageAmount, double totalPrice, int roomTypeCode, string roomTypeDescription, string roomPakages, string ratePolicy, int rateCode, int rateCodeDetail)
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
                ViewBag.Gender = user?.gender;
            }
            var b = await _authenticationManager.identificationValid();
            ViewBag.isVaild = b.isValid;
            ViewBag.isLoggedIn = b.isLoggedIn;
            return View(RoomFormDatajson);
        }
    }
}
