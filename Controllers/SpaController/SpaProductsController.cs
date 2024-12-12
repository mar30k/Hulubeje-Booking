using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.SpaModels;
using HulubejeBooking.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using HulubejeBooking.Helpers;
using HulubejeBooking.Models.HotelModels;

namespace HulubejeBooking.Controllers.SpaController
{
    public class SpaProductsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;
        private AuthenticationManager _authenticationManager;
        public SpaProductsController(IHttpContextAccessor? httpContextAccessor, AuthenticationManager authenticationManager, IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Index(int department, int childDepartment, string name, int company)
        {
            var token = "";
            CompanyDetailModel companyDetail = new() { Department = department.ToString(), CompanyCode = company, OrgOUD = childDepartment, Name = name };
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
            var loginInfo = HttpContext.Session.GetString("IsLogin");
            var login = "";
            if (loginInfo != null)
            {
                login = JsonConvert.DeserializeObject<string>(loginInfo);
            }
            HttpContext.Session.Remove("IsLogin");
            if (login != "Yes")
            {
                var paramJson = JsonConvert.SerializeObject(companyDetail);
                HttpContext.Session.SetString("spaValues", paramJson);
                if (identificationResult != null && !identificationResult.isValid && !identificationResult.isLoggedIn)
                {
                    string validation = "Spa";
                    var validationJson = JsonConvert.SerializeObject(validation);
                    HttpContext.Session.SetString("SignInInformation", validationJson);
                    TempData["ErrorMessage"] = "Please login to proceed further.";
                    return RedirectToAction("Index", "SignIn");

                }
            }
            var seatValuesJson = HttpContext.Session.GetString("spaValues");
            HttpContext.Session.Remove("spaValues");
            if (seatValuesJson != null)
            {
                companyDetail = JsonConvert.DeserializeObject<CompanyDetailModel>(seatValuesJson) ?? new CompanyDetailModel();
            }
            else
            {
                TempData["ErrorMessage"] = "Session Has Expired Please Restart the Booking Process";
                return RedirectToAction("Index", "Home");
            }
            if (HttpContext.Session.TryGetValue("GetSpaReservation", out var getspaReservationBytes))
            {
                 
                var spaReservation = Encoding.UTF8.GetString(getspaReservationBytes);
                var getspaReservation = spaReservation != null
                ? JsonConvert.DeserializeObject<HulubejeResponse<List<Categorys>>>(spaReservation) ?? new HulubejeResponse<List<Categorys>>()
                : new HulubejeResponse<List<Categorys>>();

                var product = getspaReservation?.Data?
                    .Where(c => c.Code.ToString() == companyDetail.Department)
                    .SelectMany(c => c.Children ?? new List<Child>())
                    .Where(child => child.Name == companyDetail.Name && child.Code == companyDetail.OrgOUD)
                    .FirstOrDefault();
                List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("CartItems") ?? new List<CartItem>();

                var spaProducts = new SpaProductsView
                {
                    Child = product,
                    CartItem = cart,
                    CompanyDetailModel = new CompanyDetailModel
                    {
                        CompanyCode = companyDetail.CompanyCode, 
                        Department = companyDetail.Department?.ToString()
                    }
                };

                return View(spaProducts);

            }
            else
            {
                TempData["ErrorMessage"] = "Session Has Expired Please Restart the Booking Process";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult SaveCart([FromBody] List<CartItem> cartItems)
        {
            // Save the cart items in the session
            HttpContext.Session.SetObjectAsJson("CartItems", cartItems);
            return Ok();
        }

    }
}
