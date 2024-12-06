using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.SpaModels;
using HulubejeBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using HulubejeBooking.Helpers;
using HulubejeBooking.Models.HotelModels;

namespace HulubejeBooking.Controllers.SpaController
{
    public class SchedulesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;
        private AuthenticationManager _authenticationManager;
        public SchedulesController(IHttpContextAccessor? httpContextAccessor, AuthenticationManager authenticationManager, IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Index(int company, string department)
        {
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
            if (HttpContext.Session.TryGetValue("CartItems", out var getspaReservationBytes))
            {
                List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("CartItems") ?? new List<CartItem>();
                //GetCompanySchedule companyschedule = HttpContext.Session.GetObjectFromJson<GetCompanySchedule>("companyschedule") ?? new GetCompanySchedule();

                _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var getspareservation = await GetspareservationResponseAsync(company.ToString(), department, "50", "2024-11-22T08:00:00.000", "2024-11-22T17:00:00.000",
                    token) ?? new HulubejeResponse<List<Schedules>>();
                var companyDetail = new CompanyDetailModel
                {
                    CompanyCode = company
                };
                var spareservation = JsonConvert.SerializeObject(getspareservation);
                //HttpContext.Session.SetString("GetSpaReservation", spareservation);
                var ScheduleView = new ScheduleView { CartItem = cart, Schedules = getspareservation, CompanyDetailModel = companyDetail };
                return View(ScheduleView);
            }
            else
            {
                TempData["ErrorMessage"] = "Session Has Expired Please Restart the Booking Process";
                return RedirectToAction("Index", "Home");
            }

        }
        public async Task<HulubejeResponse<List<Schedules>>> GetspareservationResponseAsync(string company, string department, string duration, 
            string startTime, string endtime, string? token)
        {
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");

            _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var getspareservation = new HulubejeResponse<List<Schedules>>();
            HttpResponseMessage getspareservationResponse = await _v7Client.GetAsync($"spareservation/available?company={company}" +
                $"&department={department}&duration={duration}&" +
                $"CompanyStartTime={startTime}&CompanyEndTime={endtime}");
            if (getspareservationResponse.IsSuccessStatusCode)
            {
                string getcompanyscheduleData = await getspareservationResponse.Content.ReadAsStringAsync();
                getspareservation = JsonConvert.DeserializeObject<HulubejeResponse<List<Schedules>>>(getcompanyscheduleData);
            }
            return getspareservation ?? new HulubejeResponse<List<Schedules>>();
        }
    }

}
