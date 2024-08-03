using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.HotelModels;
using HulubejeBooking.Models.PaymentModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace HulubejeBooking.Controllers.EventController
{
    public class EventDetailsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;
        private AuthenticationManager _authenticationManager;
        public EventDetailsController(IHttpContextAccessor? httpContextAccessor, AuthenticationManager authenticationManager, IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> IndexAsync(int orgCode, int oud, string orgTin, string description, string cityname, string name, string branchName)
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
            try
            {
                _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage getcompanyscheduleResponse = await _v7Client.GetAsync($"routing/getcompanyschedule?companyCode={orgCode}&branchCode={oud}&industryType=1989");
                var getsupplierpaymentoptions = new PaymentProcessorResponse();
                var getcompanyschedule = new GetCompanySchedule();
                var getcompanyimages = new GetCompanyImages();
                if (getcompanyscheduleResponse.IsSuccessStatusCode)
                {
                    string getcompanyscheduleData = await getcompanyscheduleResponse.Content.ReadAsStringAsync();
                    getcompanyschedule = JsonConvert.DeserializeObject<GetCompanySchedule>(getcompanyscheduleData);
                }

                HttpResponseMessage getsupplierpaymentoptionsResponse = await _v7Client.GetAsync($"payment/getsupplierpaymentoptions?code={orgCode}&branchCode={oud}");
                HttpResponseMessage getcompanyimagesResponse = await _v7Client.GetAsync($"routing/getcompanyimages?tin={orgTin}&branchCode={oud}&industryType=1989");

                if (getsupplierpaymentoptionsResponse.IsSuccessStatusCode)
                {
                    string getsupplierpaymentoptionsData = await getsupplierpaymentoptionsResponse.Content.ReadAsStringAsync();
                    getsupplierpaymentoptions = JsonConvert.DeserializeObject<PaymentProcessorResponse>(getsupplierpaymentoptionsData);
                }
                if (getcompanyimagesResponse.IsSuccessStatusCode)
                {
                    string getcompanyimagesData = await getcompanyimagesResponse.Content.ReadAsStringAsync();
                    getcompanyimages = JsonConvert.DeserializeObject<GetCompanyImages>(getcompanyimagesData);
                }

                var CompanyDetailModel = new CompanyDetailModel
                {
                    BranchName = branchName,
                    PaymentOptions = getsupplierpaymentoptions,
                    Description = description,
                    Name = name,
                    CompanySchedule = getcompanyschedule,
                    CompanyCode = orgCode,
                    OrgOUD = oud,
                    CityName = cityname,
                    ImageModel = getcompanyimages
                };
                return View(CompanyDetailModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
