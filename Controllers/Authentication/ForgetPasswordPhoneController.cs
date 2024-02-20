using HulubejeBooking.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace HulubejeBooking.Controllers.Authentication
{
    public class ForgetPasswordPhoneController : Controller
    {
        private readonly HttpClient _httpClient;
        private IHttpClientFactory _httpClientFactory;

        public ForgetPasswordPhoneController(HttpClient httpClient, IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClient;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var countryCodes = new List<CountryResponse>();
            var endPoint = "https://restcountries.com/v3.1/all";
            HttpResponseMessage response = await _httpClient.GetAsync(endPoint);
            if (response.IsSuccessStatusCode)
            {
                var respnseData = await response.Content.ReadAsStringAsync();
                countryCodes = respnseData != null ? JsonConvert.DeserializeObject<List<CountryResponse>>(respnseData) : new List<CountryResponse>();
                var ethiopia = countryCodes?.Find(c => c.Name?.Common == "Ethiopia");
                if (ethiopia != null)
                {
                    countryCodes?.Remove(ethiopia);
                    countryCodes?.Insert(0, ethiopia);
                }
            }
            return View(countryCodes);
        }
        [HttpPost]
        public async Task<IActionResult> IndexAsync(string phoneNumber)
        {
            var _client = _httpClientFactory.CreateClient("CnetHulubeje");
            var PARAM = new
            {
                latitude = 0,
                longitude = 0,
                platform = "web",
                target = "",
                username = phoneNumber,
                phoneNumber,
            };
            string jsonBody = JsonConvert.SerializeObject(PARAM);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");


            HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "/Profile/forgotPassword", content);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();

                var suthResponse = data != null ? JsonConvert.DeserializeObject<ForgetPassword>(data) : new ForgetPassword();
                var userInformation = suthResponse != null && suthResponse.IsActive;
                var key = "ForgetPassword";
                if (userInformation == true)
                {
                    HttpResponseMessage otpResponse = await _client.GetAsync(_client.BaseAddress + $"/Messaging/SendOTP?to={phoneNumber}");
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
