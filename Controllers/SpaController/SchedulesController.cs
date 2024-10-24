using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.SpaModels;
using HulubejeBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using HulubejeBooking.Helpers;

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
        public async Task<IActionResult> IndexAsync()
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

            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var getspareservation = new HulubejeResponse<List<Schedules>>();
            HttpResponseMessage getspareservationResponse = await _v7Client.GetAsync($"spareservation/available?company=122225&department=6&duration=50&" +
                $"CompanyStartTime=2024-11-22T08:00:00.000&CompanyEndTime=2024-11-22T17:00:00.000");
            if (getspareservationResponse.IsSuccessStatusCode)
            {
                string getcompanyscheduleData = await getspareservationResponse.Content.ReadAsStringAsync();
                getspareservation = JsonConvert.DeserializeObject<HulubejeResponse<List<Schedules>>>(getcompanyscheduleData);
            }
            var spareservation = JsonConvert.SerializeObject(getspareservation);
            //HttpContext.Session.SetString("GetSpaReservation", spareservation);
            var ScheduleView = new ScheduleView { CartItem = cart , Schedules = getspareservation};
            return View(ScheduleView);
        }
    }

}
