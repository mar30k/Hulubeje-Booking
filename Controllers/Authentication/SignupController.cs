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
using Tweetinvi.Core.Events;

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
                var _V7client = _httpClientFactory.CreateClient("HulubejeBooking");
                var userInfo = new
                {
                    code = person.PhoneNumber,
                    fullname = person.FirstName + person.MiddleName + person.LastName,
                    password = person.Password,
                    referenceNumber = "",
                    email = person.EmailAddress,
                    ActivityLog =new
                    {
                        latitude = 0,
                        longitude = 0,
                        platform = "web",
                        target = "",
                        code = person.PhoneNumber,
                    }
                };
                string jsonBody = JsonConvert.SerializeObject(userInfo);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _V7client.PostAsync("auth/register", content);
                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<RegisterUserResponse>(responseString);
                    if (responseData != null && responseData.IsSuccessful && responseData.Data !=null)
                    {
                        var isLoggedIn = true;
                        string isLoggedInJson = JsonConvert.SerializeObject(isLoggedIn);
                        HttpContext.Session.SetString("isLoggedIn", isLoggedInJson);

                        //var userInformation = new UserData()
                        //{
                        //    Token = person?.PhoneNumber,
                        //};
                        _authenticationManager.SignIn(responseData.Data, true);
                        TempData["InfoMessage"] = "Welcome! You Have Successfully Created Hulubeje Account";
                        return RedirectToAction("Index", "Home");
                    }
                    else if (responseData?.ErrorMessages?.Count > 0)
                    {
                        TempData["ErrorMessage"] = responseData?.ErrorMessages?.FirstOrDefault();
                        return RedirectToAction("Index", "signin");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Seems Like You Have an Account. Please Login!";
                        return RedirectToAction("Index", "signin");
                    }

                }
                else
                {
                    TempData["ErrorMessage"] = "Seems Like You Have an Account. Please Login!";
                    return RedirectToAction("Index", "signin");
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
