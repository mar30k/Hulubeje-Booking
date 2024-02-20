using HulubejeBooking.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System;
using Microsoft.CodeAnalysis;

namespace HulubejeBooking.Controllers.Authentication
{
    public class ForgetPasswordController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IHttpClientFactory _httpClientFactory;
        public ForgetPasswordController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> IndexAsync(string password, string repeatpassword)
        {
            var userDataCookie = _httpContextAccessor?.HttpContext?.Request.Cookies[CNET_WebConstants.IdentificationCookie];
            string? phoneNumber = "";
            if (!string.IsNullOrEmpty(userDataCookie))
            {
                var user = JsonConvert.DeserializeObject<UserInformation>(userDataCookie);
                phoneNumber = user != null ? user?.phoneNumber : null;
            }
            var _client = _httpClientFactory.CreateClient("CnetHulubeje");

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
                var param = new
                {
                    phoneNumber,
                    latitude = 0,
                    longitude = 0,
                    platform = "web",
                    target = "",
                    password,
                };
                string jsonBody = JsonConvert.SerializeObject(param);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + $"/Profile/updatePassword", content);
                string responseData = await response.Content.ReadAsStringAsync();
                if (Convert.ToBoolean(responseData))
                {
                    TempData["InfoMessage"] = "Successfully Changed Password!";
                    return RedirectToAction("Index", "signin");
                }
                return View();
            }
        }
    }
}
