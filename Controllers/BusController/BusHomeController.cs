using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using HulubejeBooking.Models.BusModels;
using HulubejeBooking.Models.Authentication;
namespace HulubejeBooking.Controllers.BusController
{
    public class BusHomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;

        public BusHomeController(IHttpClientFactory httpClientFactory, IHttpContextAccessor? httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
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
                busModel.Routes = routeData;
            }
            return View(busModel);
        }

    }
}
