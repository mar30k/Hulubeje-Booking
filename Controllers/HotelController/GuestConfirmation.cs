using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.HotelModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HulubejeBooking.Controllers.HotelController
{
    public class GuestConfirmation : Controller
    {
        private IHttpContextAccessor? _httpContextAccessor;
        private readonly AuthenticationManager _authenticationManager;
        public GuestConfirmation(AuthenticationManager authenticationManager, IHttpContextAccessor httpContextAccessor)
        {
            _authenticationManager = authenticationManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index(List<Guests> guests)
        {
            var RoomFormData = HttpContext.Session.GetString("RoomFormData");
            var RoomFormDatajson = new RoomFormData();

            if (RoomFormData != null)
            {
                RoomFormDatajson = JsonConvert.DeserializeObject<RoomFormData>(RoomFormData);

            }
            var roomDetails = HttpContext.Session.GetString("RoomType");
            var roomDetailsJson = new RoomType();

            if (roomDetails != null)
            {
                roomDetailsJson = JsonConvert.DeserializeObject<RoomType>(roomDetails);

            }
            var GuestModelWrapper = new GuestModelWrapper
            {
                RoomFormData = RoomFormDatajson,
                RoomType = roomDetailsJson,
                Guests  = guests
            };
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
            return View(GuestModelWrapper);
        }
    }
}
