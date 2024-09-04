using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.HotelModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;

namespace HulubejeBooking.Controllers.SpaController
{
    public class SpaController : Controller
    {
        private IHttpContextAccessor? _httpContextAccessor;
        private AuthenticationManager _authenticationManager;
        private IHttpClientFactory _httpClientFactory;

        public SpaController(IHttpContextAccessor? httpContextAccessor, AuthenticationManager? authenticationManager, IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Index()
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
            var getcompaniesbytype = new GetcompaniesbyType();
            _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage getcompaniesbytypeResponse = await _v7Client.GetAsync("routing/getcompaniesbytype?industryType=1991");
            if (getcompaniesbytypeResponse.IsSuccessStatusCode)
            {
                string getcompaniesbytypeData = await getcompaniesbytypeResponse.Content.ReadAsStringAsync();
                getcompaniesbytype = getcompaniesbytypeData != null ? JsonConvert.DeserializeObject<GetcompaniesbyType>(getcompaniesbytypeData) : new GetcompaniesbyType();
            }
            return View(getcompaniesbytype);
        }
    }
}
