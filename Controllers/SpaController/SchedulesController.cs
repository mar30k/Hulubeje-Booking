using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.SpaModels;
using HulubejeBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using HulubejeBooking.Helpers;
using HulubejeBooking.Models.HotelModels;
using DevExpress.Charts.Native;
using DevExpress.Xpo.DB;

namespace HulubejeBooking.Controllers.SpaController
{
    public class SchedulesController : Controller
    {
        private readonly MiscellaneousApiRequests _miscellaneousApiRequests;
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;
        private AuthenticationManager _authenticationManager;
        public SchedulesController(IHttpContextAccessor? httpContextAccessor, AuthenticationManager authenticationManager, IHttpClientFactory httpClientFactory, MiscellaneousApiRequests miscellaneousApiRequests)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
            _httpClientFactory = httpClientFactory;
            _miscellaneousApiRequests = miscellaneousApiRequests;
        }
        [HttpGet]
        public async Task<IActionResult> Index(DateTime date, int company, string department, int duration, string? mode = null)
        {
            if (string.IsNullOrEmpty(mode) || mode == "1")
            {
                var token = await AuthenticateAndSetViewData();
                if (string.IsNullOrEmpty(token))
                {
                    TempData["ErrorMessage"] = "Authentication failed. Please try again.";
                    return RedirectToAction("Index", "Home");
                }

                if (HttpContext.Session.TryGetValue("CartItems", out var _) &&
                    HttpContext.Session.TryGetValue("companyschedule", out var _))
                {
                    var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("CartItems") ?? new List<CartItem>();
                    var companySchedule = HttpContext.Session.GetObjectFromJson<GetCompanySchedule>("companyschedule") ?? new GetCompanySchedule();

                    var datePart = mode == "1" ? date.Date : await GetServerDatePart(token);

                    var (startDateTime, endDateTime) = GetScheduleDateTimeRange(companySchedule, datePart.Value);
                    if (string.IsNullOrEmpty(startDateTime) || string.IsNullOrEmpty(endDateTime))
                    {
                        TempData["ErrorMessage"] = "Failed to determine schedule times.";
                        return RedirectToAction("Index", "Home");
                    }

                    var schedules = await GetSchedules(company.ToString(), department, duration, startDateTime, endDateTime, token);
                    if (schedules == null)
                    {
                        TempData["ErrorMessage"] = "Failed to retrieve schedule information.";
                        return RedirectToAction("Index", "Home");
                    }

                    var companyDetail = new CompanyDetailModel
                    {
                        CompanyCode = company,
                        Department = department,
                        Duration = duration
                    };

                    var scheduleView = new ScheduleView
                    {
                        CartItem = cart,
                        Schedules = schedules,
                        CompanyDetailModel = companyDetail
                    };

                    return View(scheduleView);
                }

                TempData["ErrorMessage"] = "Session has expired. Please restart the booking process.";
                return RedirectToAction("Index", "Home");
            }

            TempData["ErrorMessage"] = "Invalid mode provided.";
            return RedirectToAction("Index", "Home");
        }

        // Helper Methods
        private async Task<string?> AuthenticateAndSetViewData()
        {
            var identificationResult = await _authenticationManager.identificationValid();
            if (identificationResult == null) return null;

            var userData = identificationResult.UserData;
            ViewBag.isVaild = identificationResult.isValid;
            ViewBag.isLoggedIn = identificationResult.isLoggedIn;
            ViewBag.FirstName = userData?.FirstName;
            ViewBag.LastName = userData?.LastName;
            ViewBag.MiddleName = userData?.MiddleName;
            ViewBag.Personalattachment = userData?.PersonalAttachment;
            ViewBag.Idnumber = userData?.IdNumber;
            ViewBag.Idtype = userData?.IdType;
            ViewBag.Dob = userData?.Dob;
            ViewBag.Idattachment = userData?.IdAttachment;
            ViewBag.PhoneNumber = userData?.Code;
            ViewBag.EmailAddress = userData?.Email;

            return userData?.Token ?? "";
        }

        private async Task<DateTime?> GetServerDatePart(string token)
        {
            string serverTimeString = (await _miscellaneousApiRequests.GetServerTime(token) ?? "").Trim('\"');
            if (DateTimeOffset.TryParse(serverTimeString, out var serverTime))
            {
                return serverTime.Date;
            }
            return null;
        }

        private static (string? StartDateTime, string? EndDateTime) GetScheduleDateTimeRange(GetCompanySchedule schedule, DateTime datePart)
        {
            string dayOfWeek = datePart.DayOfWeek.ToString().ToLower();
            var daySchedule = schedule?.Data?.Schedules?.FirstOrDefault(s => s.TimeLabel?.ToLower() == dayOfWeek);
            if (daySchedule == null || string.IsNullOrEmpty(daySchedule.StartTime) || string.IsNullOrEmpty(daySchedule.EndTime))
            {
                return (null, null);
            }

            var startTimePart = TimeSpan.Parse(daySchedule.StartTime);
            var endTimePart = TimeSpan.Parse(daySchedule.EndTime);

            return (
                new DateTimeOffset(datePart + startTimePart).ToString("yyyy-MM-ddTHH:mm:ss.fff"),
                new DateTimeOffset(datePart + endTimePart).ToString("yyyy-MM-ddTHH:mm:ss.fff")
            );
        }

        private async Task<HulubejeResponse<List<Schedules>>> GetSchedules(string company, string department, int duration, string startDateTime, string endDateTime, string token)
        {
            _httpClientFactory.CreateClient("HulubejeBooking").DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await GetspareservationResponseAsync(company, department, duration, startDateTime, endDateTime, token);
        }


        public async Task<HulubejeResponse<List<Schedules>>> GetspareservationResponseAsync(string company, string department, int duration,
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
