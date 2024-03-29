using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using HulubejeBooking.Models.BusModels;
using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Controllers.Authentication;
namespace HulubejeBooking.Controllers.BusController
{
    public class BusController : Controller
    {
        private readonly AuthenticationManager _authenticationManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;

        public BusController(IHttpClientFactory httpClientFactory, IHttpContextAccessor? httpContextAccessor,AuthenticationManager authenticationManager)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
        }

        public async Task<IActionResult> Index()
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
            HttpResponseMessage response = await busSeatLayoutClient.GetAsync("operators/getalloperators");
            var busModel = new BusModel();
            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var companyData = JsonConvert.DeserializeObject<List<CompanyModel>>(responseData);
                busModel.Company = companyData;
            }
            HttpResponseMessage routeResponse = await busSeatLayoutClient.GetAsync("routes/getallroutes");
            if(routeResponse.IsSuccessStatusCode)
            {
                string routeResponseData = await routeResponse.Content.ReadAsStringAsync();
                var routeData = JsonConvert.DeserializeObject<List<RouteModel>>(routeResponseData);
                busModel.RouteModel = routeData;
            }
            return View(busModel);
        }

    }
}
