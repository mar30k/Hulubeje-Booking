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

        public SignupController(ILogger<SignupController> logger,
            IHttpClientFactory httpClientFactory,
             HotelListBuffer hotelListBuffer)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {

            return View();
        }
        public async Task<IActionResult> SignUpVerificationAsync(PersonModel person)
        {
            if (ModelState.IsValid)
            {
                var _client = _httpClientFactory.CreateClient("CnetHulubeje");

                person.personCode = person.phoneNumber;
                var param = new
                {
                    userId = person.phoneNumber
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
                        var phone = person.phoneNumber;
                        HttpResponseMessage otpResponse = await _client.GetAsync(_client.BaseAddress + $"/Messaging/SendOTP?to={phone}");
                        if (otpResponse.IsSuccessStatusCode)
                        {
                            string dataRespponse = await otpResponse.Content.ReadAsStringAsync();
                            var messageResponse = JsonConvert.DeserializeObject<MessageResponse>(dataRespponse);
                            person.messageResponse = messageResponse;
                            var PersonJson = JsonConvert.SerializeObject(person);
                            HttpContext.Session.SetString("UserInfo", PersonJson);

                        }
                    }
                }
                return RedirectToAction("Index", "Otp");
            }
            else
            {
                return View("Index", person);
            }
            
        }
    }
}
