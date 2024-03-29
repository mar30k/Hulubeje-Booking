using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Net.Http;
using Newtonsoft.Json;
using HulubejeBooking.Models.BusModels;
using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Controllers.Authentication;

namespace HulubejeBooking.Controllers.BusController
{
    public class RouteScheduleController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;
        private readonly AuthenticationManager _authenticationManager;

        public RouteScheduleController(IHttpClientFactory httpClientFactory, IHttpContextAccessor? httpContextAccessor,AuthenticationManager authenticationManager)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
        }
        public async  Task<IActionResult> Index(string depature, string destination, DateTime travelDate)
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
            var busSeatLayoutClient = _httpClientFactory.CreateClient("BusBooking");
            HttpResponseMessage response = await busSeatLayoutClient.GetAsync($"routeschedule/getschedulesbyroute?route={destination}&date={travelDate}");
            if(response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var scheduleData = responseData != null ? JsonConvert.DeserializeObject<List<VwRouteSchedule>>(responseData) : new List<VwRouteSchedule>();
                if(scheduleData != null )
                {
                    foreach (var schedule in scheduleData)
                    {
                        schedule.DepatureCity = depature;
                    }
                }
                return View(scheduleData);
            }
            return View(null);
        }
    }
}