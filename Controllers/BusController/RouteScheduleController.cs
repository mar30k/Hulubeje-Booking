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
        public async  Task<IActionResult> Index(string depature, string destination, DateTime travelDate, int? operatorId, string option)
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

            string apiUrl;

            if (option == "detail")
            {
                apiUrl = $"routeschedule/getschedulesbyrouteandoperator?route={destination}&date={travelDate}&operatorid={operatorId}";
            }
            else
            {
                apiUrl = $"routeschedule/getschedulesbyroute?route={destination}&date={travelDate}";
            }

            var scheduleData = await GetScheduleData(apiUrl, depature);
            return View(scheduleData);
        }

        private async Task<List<VwRouteSchedule>> GetScheduleData(string apiUrl, string depature)
        {
            try
            {
                var busSeatLayoutClient = _httpClientFactory.CreateClient("BusBooking");
                HttpResponseMessage response = await busSeatLayoutClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    var scheduleData = JsonConvert.DeserializeObject<List<VwRouteSchedule>>(responseData);
                    if (scheduleData != null)
                    {
                        foreach (var schedule in scheduleData)
                        {
                            schedule.DepatureCity = depature;
                        }
                    }
                    return scheduleData ?? new List<VwRouteSchedule>();
                }
                else
                {
                    return new List<VwRouteSchedule>();
                }
            }
            catch (Exception ex) { 
                Console.WriteLine(ex.Message);
            }
            return new List<VwRouteSchedule>();
        }
    }
}