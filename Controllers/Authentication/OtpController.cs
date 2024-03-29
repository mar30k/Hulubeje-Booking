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
            var _V7client = _httpClientFactory.CreateClient("HulubejeBooking");
            var data = HttpContext.Session.GetString("OtpMessageResponse");
            var forgetPassword = HttpContext.Session.GetString("ForgetPassword") != null;
            var newData = data != null ? JsonConvert.DeserializeObject<MessageResponse>(data) : new MessageResponse() ;
            var code = newData?.code;
            var to = newData?.to;
            var vc = newData?.verificationId;
            var messageId = newData?.messageId;
            if (forgetPassword)
            {
                if (trimmedCode == code)
                {
                    HttpResponseMessage response = await _V7client.GetAsync($"messaging/verifyotp?to={to}&vc={vc}&code={code}&messageId={messageId}");
                    string responseData = await response.Content.ReadAsStringAsync();
                    var verificationData = JsonConvert.DeserializeObject<VerificationResponse>(responseData);
                    var json = JsonConvert.SerializeObject(verificationData);
                    if (verificationData?.IsVerified == true)
                    {
                         HttpContext.Session.SetString("VerificationResponse", json);
                        return RedirectToAction("Index", "ForgetPassword");
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
            else
            {
                if (trimmedCode == code)
                {
                    HttpResponseMessage response = await _V7client.GetAsync($"messaging/verifyotp?to={to}&vc={vc}&code={code}&messageId={messageId}");
                    string responseData = await response.Content.ReadAsStringAsync();
                    var verificationData = JsonConvert.DeserializeObject<VerificationResponse>(responseData);
                    var json = JsonConvert.SerializeObject(verificationData);
                    if (verificationData?.IsVerified == true)
                    {
                        HttpContext.Session.SetString("VerificationResponse", json);
                        return RedirectToAction("Index", "Signup");
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
}
