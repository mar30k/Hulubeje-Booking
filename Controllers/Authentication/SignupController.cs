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
        private readonly object JsonRequestBehavior;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HotelListBuffer _hotelListBuffer;

        public SignupController(ILogger<SignupController> logger,
            IHttpClientFactory httpClientFactory,
             HotelListBuffer hotelListBuffer)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _hotelListBuffer = hotelListBuffer;
        }

        public IActionResult Index()
        {

            return View();
        }
        public async Task<IActionResult> SignUpVerificationAsync(PersonModel person)
        {
            var _client = _httpClientFactory.CreateClient("CnetHulubeje");

            person.personCode = person.phoneNumber;
            var param = new
            {
                userId = person.phoneNumber
            };
            string jsonBody = JsonConvert.SerializeObject(param);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var suthResponse = new List<UserResponse>();

            HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "/Profile/OauthAuthenticateUser", content);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();

                suthResponse = JsonConvert.DeserializeObject<List<UserResponse>>(data);

                object statusCodeAtIndexZero = (object)suthResponse[0]  .userInformation;

                if (statusCodeAtIndexZero != null)
                {
                    return RedirectToAction("Index", "SignIn");
                }
                else
                {
                    var phone = person.phoneNumber;
                    HttpResponseMessage otpResponse = await _client.GetAsync(_client.BaseAddress + $"/Messaging/SendOTP?to={phone}");
                    if (otpResponse.IsSuccessStatusCode)
                    {
                        string dataRespponse = await otpResponse.Content.ReadAsStringAsync();
                        var messageResponse= JsonConvert.DeserializeObject<MessageResponse>(dataRespponse);
                        person.messageResponse = messageResponse;
                        var PersonJson = JsonConvert.SerializeObject(person);
                        HttpContext.Session.SetString("UserInfo", PersonJson);

                    }
                }
            }
            return RedirectToAction("Index", "Otp");
        }
    }
}
