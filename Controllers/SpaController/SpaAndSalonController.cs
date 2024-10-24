using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.EventModels;
using HulubejeBooking.Models;
using HulubejeBooking.Models.HotelModels;
using HulubejeBooking.Models.PaymentModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using HulubejeBooking.Models.SpaModels;
using DevExpress.PivotGrid.PivotTable;
using System.Text;

namespace HulubejeBooking.Controllers.SpaController
{
    public class SpaAndSalonController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;
        private AuthenticationManager _authenticationManager;
        public SpaAndSalonController(IHttpContextAccessor? httpContextAccessor, AuthenticationManager authenticationManager, IHttpClientFactory httpClientFactory)
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
            _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var getspareservation = new HulubejeResponse<List<Categorys>>();
            HttpResponseMessage getspareservationResponse = await _v7Client.GetAsync($"spareservation/get?company={company}&branch={branch}");
            if (getspareservationResponse.IsSuccessStatusCode)
            {
                string getcompanyscheduleData = await getspareservationResponse.Content.ReadAsStringAsync();
                getspareservation = JsonConvert.DeserializeObject<HulubejeResponse<List<Categorys>>>(getcompanyscheduleData);
            }
            var spareservation = JsonConvert.SerializeObject(getspareservation);
            HttpContext.Session.SetString("GetSpaReservation", spareservation);
            
            return View(getspareservation);
        }
    }
}
