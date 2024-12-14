using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.HotelModels;
using HulubejeBooking.Models.PaymentModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;

namespace HulubejeBooking.Controllers.HotelController
{
    public class HotelDetailController : Controller
    {
        private readonly AuthenticationManager _authenticationManager;
        private readonly IHttpClientFactory _httpClientFactory;
        public HotelDetailController(AuthenticationManager authenticationManager, IHttpClientFactory httpClientFactory)
        {
            _authenticationManager = authenticationManager;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Index(int orgCode, int oud, string orgTin, string description, string cityName, string name, string branchName)
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
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            var b = await _authenticationManager.identificationValid();
            string? token = b.UserData.Token;
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
                    CityName = cityName,
                    ImageModel = getcompanyimages
                };
                var HotelDetailJson = JsonConvert.SerializeObject(CompanyDetailModel);
                return View(CompanyDetailModel);
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }
    }
}
