using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.SpaModels;
using HulubejeBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using HulubejeBooking.Models.EventModels;
using HulubejeBooking.Models.HotelModels;
using HulubejeBooking.Helpers;

namespace HulubejeBooking.Controllers.EventController
{
    public class EventsListController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly AuthenticationManager _authenticationManager;

        public EventsListController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, AuthenticationManager authenticationManager )
        {
            _authenticationManager = authenticationManager;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index(int code, int articleCode, string name, int organizer)
        {
            CompanyDetailModel companyDetail = new() { Code = code, Organizer = organizer, ArticleCode = articleCode, Name = name };

            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            var token = "";
            var identificationResult = await _authenticationManager.identificationValid();
            if (identificationResult != null)
            {
                token = identificationResult.UserData.Token;
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
            var selectedEvent = new EventResponse();
            if (HttpContext.Session.TryGetValue("EventResponse", out var eventResponseBytes))
            {

                var eventResponseString = Encoding.UTF8.GetString(eventResponseBytes); 
                var eventResponse = eventResponseString != null
                ? JsonConvert.DeserializeObject<HulubejeResponse<List<EventResponse>>>(eventResponseString) ?? new HulubejeResponse<List<EventResponse>>()
                : new HulubejeResponse<List<EventResponse>>();
                selectedEvent = eventResponse?.Data?
                    .Where(c => c.Code == companyDetail?.Code && c.Organizer.ToString() == companyDetail?.Organizer?.ToString()).FirstOrDefault();
            }
            else
            {
                TempData["ErrorMessage"] = "Session Has Expired Please Restart the Booking Process";
                return RedirectToAction("Index", "Home");
            }   
            return View(selectedEvent);
        }

        [HttpPost]
        public IActionResult SaveCart([FromBody] List<CartItem> cartItems)
        {
            // Save the cart items in the session
            HttpContext.Session.SetObjectAsJson("EventCartItems", cartItems);
            return Ok();
        }
    }
}
