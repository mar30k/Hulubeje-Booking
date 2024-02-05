using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Net.Http;
using Newtonsoft.Json;
using HulubejeBooking.Models.BusModels;
using HulubejeBooking.Models.Authentication;

namespace HulubejeBooking.Controllers.BusController
{
    public class BusScheduleController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;

        public BusScheduleController(IHttpClientFactory httpClientFactory, IHttpContextAccessor? httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }
        public async  Task<IActionResult> BusScheduleview(string depature,string destination, DateTime travelDate)
        {
            var userDataCookie = _httpContextAccessor?.HttpContext?.Request.Cookies[CNET_WebConstants.IdentificationCookie];
            if (!string.IsNullOrEmpty(userDataCookie))
            {
                var user = JsonConvert.DeserializeObject<UserInformation>(userDataCookie);
                ViewBag.UserName = user?.firstName;
                ViewBag.LastName = user?.lastName;
                ViewBag.MiddleName = user?.middleName;
                ViewBag.Image = user?.personalattachment;
                ViewBag.SuccessCode = user?.successCode;
                ViewBag.Inumber = user?.idnumber;
                ViewBag.Idtype = user?.idtype;
                ViewBag.Dob = user?.dob;
                ViewBag.Idattachment = user?.idattachment;
                ViewBag.PhoneNumber = user?.phoneNumber;
                ViewBag.EmailAddress = user?.emailAddress;
            }
            var busSeatLayoutClient = _httpClientFactory.CreateClient("BusBooking");
            HttpResponseMessage response = await busSeatLayoutClient.GetAsync($"routeschedule/getschedulesbyroute?route={destination}&date={travelDate}");
            if(response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var scheduleData = JsonConvert.DeserializeObject<List<VwRouteSchedule>>(responseData);
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