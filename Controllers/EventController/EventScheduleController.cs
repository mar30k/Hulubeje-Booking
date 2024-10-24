using DevExpress.PivotGrid.PivotTable;
using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models;
using HulubejeBooking.Models.EventModels;
using HulubejeBooking.Models.HotelModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace HulubejeBooking.Controllers.EventController
{
	public class EventScheduleController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private IHttpContextAccessor? _httpContextAccessor;
		private AuthenticationManager _authenticationManager;
		public EventScheduleController(IHttpContextAccessor? httpContextAccessor, AuthenticationManager authenticationManager, IHttpClientFactory httpClientFactory)
		{
			_httpContextAccessor = httpContextAccessor;
			_authenticationManager = authenticationManager;
			_httpClientFactory = httpClientFactory;
		}
		public async Task<IActionResult> Index(int branch, int company)
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
			var eventResponse = new HulubejeResponse<EventModel>();
            _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage getEvent = await _v7Client.GetAsync($"event/get?company={company}&branch={branch}");
            if (getEvent.IsSuccessStatusCode)
            {
                string getEventData = await getEvent.Content.ReadAsStringAsync();
				eventResponse = JsonConvert.DeserializeObject<HulubejeResponse<EventModel>>(getEventData);
            }
            return View(eventResponse);
		}
	}
}
