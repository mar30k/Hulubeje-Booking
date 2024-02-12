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
            var countryCodes = new List<CountryResponse>();
            var endPoint = "https://restcountries.com/v3.1/all";
            HttpResponseMessage response = await _httpClient.GetAsync(endPoint);
            if (response.IsSuccessStatusCode)
            {
                var respnseData = await response.Content.ReadAsStringAsync();
                countryCodes = JsonConvert.DeserializeObject<List<CountryResponse>>(respnseData);
                var ethiopia = countryCodes?.Find(c => c.Name?.Common == "Ethiopia");
                if (ethiopia != null)
                {
                    countryCodes?.Remove(ethiopia);
                    countryCodes?.Insert(0, ethiopia);
                }
            }
            return View(countryCodes);
        }
        public async Task<IActionResult> AuthenticatePhone(string phoneNumber)
        {

            var _client = _httpClientFactory.CreateClient("CnetHulubeje");
            var param = new
            {
                userId = phoneNumber,
            };
            string jsonBody = JsonConvert.SerializeObject(param);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");


            HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "/Profile/OauthAuthenticateUser", content);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();

                var suthResponse = JsonConvert.DeserializeObject<List<UserResponse>>(data);

                var userInformation = suthResponse?[0].userInformation;

                if (userInformation != null)
                {
                    TempData["InfoMessage"] = "Account already exists. Please sign in.";
                    return RedirectToAction("Index", "SignIn");
                }
                else
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
                    }
                }
            }
            return RedirectToAction("Index", "Otp");
        }
    }
}
