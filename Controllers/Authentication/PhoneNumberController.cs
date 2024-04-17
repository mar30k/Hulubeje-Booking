using HulubejeBooking.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System;
using System.Net.Http;

namespace HulubejeBooking.Controllers.Authentication
{
    public class PhoneNumberController : Controller
    {
        private readonly HttpClient _httpClient;
        private IHttpClientFactory _httpClientFactory;

        public PhoneNumberController(HttpClient httpClient, IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClient;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Index()
        {
            List<CountryResponse> sortedCountryCodes = new();
            var endPoint = "https://restcountries.com/v3.1/all";
            HttpResponseMessage response = await _httpClient.GetAsync(endPoint);
            if (response.IsSuccessStatusCode)
            {
                var respnseData = await response.Content.ReadAsStringAsync();
                var countryCodes = respnseData != null ? JsonConvert.DeserializeObject<List<CountryResponse>>(respnseData) : new List<CountryResponse>();
                sortedCountryCodes = countryCodes != null ? countryCodes.OrderBy(c => c.Name?.Common).ToList() : new List<CountryResponse>();
                var ethiopia = sortedCountryCodes?.Find(c => c.Name?.Common == "Ethiopia");
                if (ethiopia != null)
                {
                    sortedCountryCodes?.Remove(ethiopia);
                    sortedCountryCodes?.Insert(0, ethiopia);
                }
            }
            return View(sortedCountryCodes);
        }
        public async Task<IActionResult> AuthenticatePhone(string phoneNumber)
        {
            var _V7client = _httpClientFactory.CreateClient("HulubejeBooking");
            var PARAM = new
            {
                latitude = 0,
                longitude = 0,
                platform = "web",
                target = "",
                code = phoneNumber
            };
            string jsonBody = JsonConvert.SerializeObject(PARAM);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            int index = phoneNumber.IndexOf("+251");
            string? code;
            if (index != -1)
            {
                code = phoneNumber.Substring(0, index) + "0" + phoneNumber[(index + 4)..];
            }
            else
            {
                code = phoneNumber;
            }
            HttpResponseMessage response = await _V7client.PostAsync($"auth/userexists?code={code}", content);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();

                var suthResponse = data != null ? JsonConvert.DeserializeObject<ChangePasswordResponse>(data) : new ChangePasswordResponse();

                var userInformation = suthResponse != null && suthResponse.Data;

                if (userInformation)
                {
                    TempData["InfoMessage"] = "Account already exists. Please sign in.";
                    return RedirectToAction("Index", "SignIn");
                }
                else
                {
                    HttpResponseMessage otpResponse = await _V7client.GetAsync($"messaging/sendotp?to={phoneNumber}");
                    if (otpResponse.IsSuccessStatusCode)
                    {
                        string dataRespponse = await otpResponse.Content.ReadAsStringAsync();
                        var otpMessage = JsonConvert.DeserializeObject<MessageResponse>(dataRespponse);
                        var PersonJson = JsonConvert.SerializeObject(otpMessage);
                        HttpContext.Session.SetString("OtpMessageResponse", PersonJson);
                        var PhoneNumber = JsonConvert.SerializeObject(phoneNumber);
                        HttpContext.Session.SetString("UserPohneNumber", PhoneNumber);
                        return RedirectToAction("Index", "Otp");
                    }
                }
            }
            return RedirectToAction("Index", "Otp");

        }
        public async Task<IActionResult> UserResponse (string phoneNumber)
        {
            var _V7client = _httpClientFactory.CreateClient("HulubejeBooking");
            var PARAM = new
            {
                latitude = 0,
                longitude = 0,
                platform = "web",
                target = "",
                code = phoneNumber
            };
            string jsonBody = JsonConvert.SerializeObject(PARAM);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            int index = phoneNumber.IndexOf("+251");
            string? code;
            if (index != -1)
            {
                code = phoneNumber[..index] + "0" + phoneNumber[(index + 4)..];
            }
            else
            {
                code = phoneNumber;
            }
            HttpResponseMessage response = await _V7client.PostAsync($"auth/userexists?code={code}", content);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();

                var suthResponse = data != null ? JsonConvert.DeserializeObject<ChangePasswordResponse>(data) : new ChangePasswordResponse();
                var userInformation = suthResponse != null && suthResponse.Data;
                var key = "ForgetPassword";
                if (userInformation == true)
                {
                    HttpResponseMessage otpResponse = await _V7client.GetAsync($"messaging/sendotp?to={phoneNumber}");
                    if (otpResponse.IsSuccessStatusCode)
                    {
                        string dataRespponse = await otpResponse.Content.ReadAsStringAsync();
                        var otpMessage = JsonConvert.DeserializeObject<MessageResponse>(dataRespponse);
                        var PersonJson = JsonConvert.SerializeObject(otpMessage);
                        HttpContext.Session.SetString("OtpMessageResponse", PersonJson);
                        var PhoneNumber = JsonConvert.SerializeObject(phoneNumber);
                        HttpContext.Session.SetString("UserPohneNumber", PhoneNumber);
                        HttpContext.Session.SetString("ForgetPassword", key);
                        return RedirectToAction("Index", "Otp");
                    }
                }
            }
            return RedirectToAction("Index", "Otp");
        }
    }
}
