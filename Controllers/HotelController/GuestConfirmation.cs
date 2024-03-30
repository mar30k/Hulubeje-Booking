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
        public async Task<IActionResult> Index(List<Guests> guests, string? specialRequirement)
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
                Guests  = guests,
                SpecialRequirement = specialRequirement
            };
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
            return View(GuestModelWrapper);
        }
    }
}
