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
        public async  Task<IActionResult> Index(string depature,string destination, DateTime travelDate)
        {
            var userDataCookie = _httpContextAccessor?.HttpContext?.Request.Cookies[CNET_WebConstants.IdentificationCookie];
            if (!string.IsNullOrEmpty(userDataCookie))
            {
                var user = userDataCookie != null ? JsonConvert.DeserializeObject<UserInformation>(userDataCookie) : new UserInformation();
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
            }
            var identificationResult = await _authenticationManager.identificationValid();
            ViewBag.isVaild = identificationResult.isValid;
            ViewBag.isLoggedIn = identificationResult.isLoggedIn;
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