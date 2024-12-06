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
            if (HttpContext.Session.TryGetValue("GetSpaReservation", out var getspaReservationBytes))
            {
                 
                var spaReservation = Encoding.UTF8.GetString(getspaReservationBytes);
                var getspaReservation = spaReservation != null
                ? JsonConvert.DeserializeObject<HulubejeResponse<List<Categorys>>>(spaReservation) ?? new HulubejeResponse<List<Categorys>>()
                : new HulubejeResponse<List<Categorys>>();

                var product = getspaReservation?.Data?
                    .Where(c => c.Code == department)
                    .SelectMany(c => c.Children ?? new List<Child>())
                    .Where(child => child.Name == name && child.Code == childDepartment)
                    .FirstOrDefault();
                List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("CartItems") ?? new List<CartItem>();

                var spaProducts = new SpaProductsView
                {
                    Child = product,
                    CartItem = cart,
                    CompanyDetailModel = new CompanyDetailModel
                    {
                        CompanyCode = company, 
                        Department = department.ToString()
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
