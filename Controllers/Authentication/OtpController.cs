using HulubejeBooking.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace HulubejeBooking.Controllers.Authentication
{
    public class OtpController : Controller
    {
        private readonly ILogger<OtpController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public OtpController(ILogger<OtpController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> VerifyOtp(string otpCode)
        {
            string? trimmedCode = "";
            if (otpCode !=null){
                 trimmedCode = otpCode.Trim();
            }
            var _client = _httpClientFactory.CreateClient("CnetHulubeje");
            var data = HttpContext.Session.GetString("UserInfo");
            var newData = JsonConvert.DeserializeObject<PersonModel>(data);
            var code = newData?.messageResponse?.code;
            var to = newData?.messageResponse?.to;
            var vc = newData?.messageResponse?.verificationId;

            if (trimmedCode == code)
            {
                HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + $"/Messaging/VerifyOTP?to={to}&vc={vc}&code={code}");
                string responseData = await response.Content.ReadAsStringAsync();
                var verificationData = JsonConvert.DeserializeObject<VerifyResponse>(responseData);

                if (verificationData?.isVerified == true)
                {
                    return RedirectToAction("Index", "RegisterUser");
                }
                else
                {
                    TempData["ErrorMessage"] = "Coundn't verify code";
                    return RedirectToAction("Index", "Otp");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Incorrect OTP. Please try again.";
                return RedirectToAction("Index", "Otp");
            }
        }

    }
}
