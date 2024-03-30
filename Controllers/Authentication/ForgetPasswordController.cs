using HulubejeBooking.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System;
using Microsoft.CodeAnalysis;
using NuGet.Common;
using System.Net.Http.Headers;

namespace HulubejeBooking.Controllers.Authentication
{
    public class ForgetPasswordController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IHttpClientFactory _httpClientFactory;
        private AuthenticationManager _authenticationManager;

        public ForgetPasswordController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, AuthenticationManager authenticationManager)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
        }
        public IActionResult Index()
        {
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> IndexAsync(string password, string repeatpassword)
        {
            var _V7client = _httpClientFactory.CreateClient("HulubejeBooking");

            if (password.Length < 6 || repeatpassword.Length < 6)
            {
                TempData["ErrorMessage"] = "password lenght should be atleast 6 characters ";
                return View();
            }
            else if (password != repeatpassword)
            {
                TempData["ErrorMessage"] = "passwords should match!";
                return View();
            }
            else
            {
                var phoneNumber = HttpContext.Session.GetString("UserPohneNumber");

                var userObject = new
                {
                    code = phoneNumber,
                    password,
                    ActivityLog = new
                    {
                        code = "",
                        target = "",
                        platform = "Web",
                        latitude = 0,
                        longitude = 0,
                        appVersion = "2.0.1+65"
                    }
                };

                string jsonBody = JsonConvert.SerializeObject(userObject);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                if (HttpContext.Session.TryGetValue("VerificationResponse", out var verificationBytes)) {
                    var verificationstring = Encoding.UTF8.GetString(verificationBytes);
                    var verificationJson= verificationstring!=null ?JsonConvert.DeserializeObject<VerificationResponse>(verificationstring): new VerificationResponse();
                    var token = verificationJson?.Token;
                    _V7client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    HttpResponseMessage response = await _V7client.PostAsync($"auth/changepassword", content);
                    string responseData = await response.Content.ReadAsStringAsync();
                    var changePasswordResponse = JsonConvert.DeserializeObject<ChangePasswordResponse>(responseData);
                    if (Convert.ToBoolean(changePasswordResponse?.Data))
                    {
                        TempData["InfoMessage"] = "Successfully Changed Password!";
                        return RedirectToAction("Index", "home");
                    }
                    else if (changePasswordResponse?.ErrorMessages?.Count > 0){
                        TempData["ErrorMessage"] = changePasswordResponse?.ErrorMessages[0];
                        return View();
                    }
                    return View();
                }
                else
                {
                    TempData["ErrorMessage"] = "Session Has Expired Please Restart the Booking Process";
                    return RedirectToAction("Index", "Home");
                }
            }
        }
    }
}
