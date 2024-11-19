using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Helpers;
using HulubejeBooking.Models.SpaModels;
using Microsoft.AspNetCore.Mvc;
using HulubejeBooking.Models.CInemaModels;
using HulubejeBooking.Models.PaymentModels;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using DevExpress.Pdf.Native.BouncyCastle.Asn1.Ocsp;
namespace HulubejeBooking.Controllers.SpaController
{
    public class Spabill : Controller
    {
        
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;
        private AuthenticationManager _authenticationManager;
        public Spabill(IHttpContextAccessor? httpContextAccessor, AuthenticationManager authenticationManager, IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Index(string phone, string name, int code)
        {
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");

            var token = "";
            var number = "";
            var identificationResult = await _authenticationManager.identificationValid();
            if (identificationResult != null)
            {
                token = identificationResult.UserData.Token;
                number = identificationResult.UserData.Code;
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
            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("CartItems") ?? new List<CartItem>();
            var lineItems = cart.Select(item => new SpaLineItem
            {
                Name = item.Name,
                Article = item.Code, 
                UnitAmount = item.Price,
                Quantity = item.Quantity,
                Parent = null,
                Uom= 0,
                Note = ""

            }).ToList();
            var activityLog = new ActivityLog
            {
                Code = number
            };
            var payload = new
            {
                ActivityLog = activityLog,
                LineItems = lineItems,
                Code = code,
            };
            var calculatedBill = new Bill();

            string jsonBody = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _v7Client.PostAsync("lineitem/calculate", content); 
            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                calculatedBill = responseData!=null ? JsonConvert.DeserializeObject<Bill>(responseData) : new Bill();
            }
            return View(calculatedBill);
        }
    }
}
