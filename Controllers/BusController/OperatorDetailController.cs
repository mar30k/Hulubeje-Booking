using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.BusModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HulubejeBooking.Controllers.BusController
{
    public class OperatorDetailController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;
        private readonly AuthenticationManager _authenticationManager;

        public OperatorDetailController(IHttpClientFactory httpClientFactory, IHttpContextAccessor? httpContextAccessor, AuthenticationManager authenticationManager)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
        }
        public async Task<IActionResult> IndexAsync(string operatorTin, string operatorName,int operatorId)
        {
            var identificationResult = await _authenticationManager.identificationValid();
            if (identificationResult != null)
            {
                ViewBag.isVaild = identificationResult.isValid;
                ViewBag.isLoggedIn = identificationResult.isLoggedIn;
                ViewBag.FirstName = identificationResult?.UserData?.FirstName;
                ViewBag.LastName = identificationResult?.UserData?.LastName;
                ViewBag.MiddleName = identificationResult?.UserData?.MiddleName;
                ViewBag.Personalattachment = identificationResult?.UserData?.PersonalAttachment;
                ViewBag.Idnumber = identificationResult?.UserData?.IdNumber;
                ViewBag.Idtype = identificationResult?.UserData?.IdType;
                ViewBag.Dob = identificationResult?.UserData?.Dob;
                ViewBag.Idattachment = identificationResult?.UserData?.IdAttachment;
                ViewBag.PhoneNumber = identificationResult?.UserData?.Code;
                ViewBag.EmailAddress = identificationResult?.UserData?.Email;
            }
            var busSeatLayoutClient = _httpClientFactory.CreateClient("BusBooking");
            var companyModel = new CompanyModel
            {
                Id = operatorId,
                TIN = operatorTin,
                CompanyName = operatorName,
            };
            var busModel = new BusModel
            {
                Company = new List<CompanyModel>(),
                RouteModel = new List<RouteModel>()
            };

            busModel?.Company.Add(companyModel);
            HttpResponseMessage routeResponse = await busSeatLayoutClient.GetAsync("routes/getallroutes");
            if (routeResponse.IsSuccessStatusCode)
            {
                string routeResponseData = await routeResponse.Content.ReadAsStringAsync();
                var routeData = JsonConvert.DeserializeObject<List<RouteModel>>(routeResponseData);
                if (routeData != null)
                {
                    if (busModel!=null)
                    {
                        busModel.RouteModel = new List<RouteModel>();
                        busModel.RouteModel.AddRange(routeData);
                    }
                }
            }
            return View(busModel);
        }
    }
}
