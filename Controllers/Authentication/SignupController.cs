using HulubejeBooking.Controllers.HotelController;
using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.HotelModels;
using HulubejeBooking.Models.PaymentModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Net.Http;
using System.Net.Mail;
using System.Text;

namespace HulubejeBooking.Controllers.Authentication
{
    public class SignupController : Controller
    {
        private readonly ILogger<SignupController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationManager _authenticationManager;

        public SignupController(ILogger<SignupController> logger,
            IHttpClientFactory httpClientFactory, AuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var identificationResult = await _authenticationManager.identificationValid();
            ViewBag.isVaild = identificationResult.isValid;
            ViewBag.isLoggedIn = identificationResult.isLoggedIn;
            if (identificationResult.isLoggedIn || identificationResult.isValid)
            {
                TempData["ErrorMessage"] = "You are already logged in!";
                return RedirectToAction("Index", "home");
            }
            var data = HttpContext.Session.GetString("UserPohneNumber");

            if (!string.IsNullOrEmpty(data) && data.StartsWith('"') && data.EndsWith('"'))
            {
                data = data.Substring(1, data.Length - 2);
            }

            var person = new PersonModel
            {
                PhoneNumber = data
            };

            return View(person);
        }

        public async Task<IActionResult> RegisterUser(PersonModel person)
        {
            if (ModelState.IsValid)
            {
                var _client = _httpClientFactory.CreateClient("CnetHulubeje");
                person.ConfirmPassword = "";
                string jsonBody = JsonConvert.SerializeObject(person);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "/Profile/createUser", content);
                string responseData = await response.Content.ReadAsStringAsync();
                if (responseData != null && Convert.ToBoolean(responseData))
                {
                    var isLoggedIn = true;
                    string isLoggedInJson = JsonConvert.SerializeObject(isLoggedIn);
                    HttpContext.Session.SetString("isLoggedIn", isLoggedInJson);

                    var userInformation = new UserInformation()
                    {
                        code = person?.PhoneNumber,
                    };
                    _authenticationManager.SignIn(userInformation, true);
                    TempData["InfoMessage"] = "Welcome! You Have Successfully Created Hulubeje Account";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = "It seems like this phone number is already registered. Please use a different number or sign in.";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                var data = HttpContext.Session.GetString("UserPohneNumber");

                if (!string.IsNullOrEmpty(data) && data.StartsWith('"') && data.EndsWith('"'))
                {
                    data = data.Substring(1, data.Length - 2);
                }
                person.PhoneNumber = data;
                return View("Index", person);
            }

        }
    }
}
