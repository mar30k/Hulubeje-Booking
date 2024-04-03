using Microsoft.AspNetCore.Mvc;
using HulubejeBooking.Models.Authentication;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
namespace HulubejeBooking.Controllers.Authentication
{
    public class ChangePasswordController : Controller
    {
        private readonly AuthenticationManager _authenticationManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor? _httpContextAccessor;
        public ChangePasswordController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, AuthenticationManager authenticationManager)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var identificationResult = await _authenticationManager.identificationValid();
            if (!(identificationResult.isLoggedIn || identificationResult.isValid))
            {
                TempData["ErrorMessage"] = "Please Login First";
                return RedirectToAction("Index", "home");
            }
            if (identificationResult != null)
            {
                ViewBag.FirstName = identificationResult?.UserData.FirstName;
                ViewBag.LastName = identificationResult?.UserData.LastName;
                ViewBag.MiddleName = identificationResult?.UserData.MiddleName;
                ViewBag.Personalattachment = identificationResult?.UserData.PersonalAttachment;
                ViewBag.Idnumber = identificationResult?.UserData.IdNumber;
                ViewBag.Idtype = identificationResult?.UserData.IdType;
                ViewBag.Dob = identificationResult?.UserData.Dob;
                ViewBag.Idattachment = identificationResult?.UserData.IdAttachment;
                ViewBag.PhoneNumber = identificationResult?.UserData.Code;
                ViewBag.EmailAddress = identificationResult?.UserData.Email;
            }
            ViewBag.isVaild = identificationResult?.isValid;
            ViewBag.isLoggedIn = identificationResult?.isLoggedIn;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(ChangePasswordModel changePassword)
        {
            var _V7client = _httpClientFactory.CreateClient("HulubejeBooking");
            string? phoneNumber;
            if (changePassword?.RepeatPassword?.Length < 6)
            {
                TempData["ErrorMessage"] = "Incorrect Old Password!";
                return View();
            }
            else if (changePassword?.Newpassword?.Length < 6 || changePassword?.RepeatPassword?.Length < 6)
            {
                TempData["ErrorMessage"] = "password lenght should be atleast 6 characters ";
                return View();
            }
            else if (changePassword?.RepeatPassword != changePassword?.Newpassword)
            {
                TempData["ErrorMessage"] = "new passwords should match!";
                return View();
            }
            else
            {
                var identificationResult = await _authenticationManager.identificationValid();
                phoneNumber = identificationResult.UserData.Code;
                var param = new
                {
                    code = phoneNumber,
                    password = changePassword?.Oldpassword,
                };
                var param2 = new
                {
                    code = phoneNumber,
                    password = changePassword?.Newpassword,
                };
                string jsonBody = JsonConvert.SerializeObject(param);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                string jsonBody2 = JsonConvert.SerializeObject(param2);
                var content2 = new StringContent(jsonBody2, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _V7client.PostAsync($"auth/login", content);
                
                if (response.IsSuccessStatusCode) {

                    string loginResponse = await response.Content.ReadAsStringAsync();
                    var userexistsDataResponse = JsonConvert.DeserializeObject<LoginAuthentication>(loginResponse);
                    if (loginResponse != null && userexistsDataResponse?.Data != null && userexistsDataResponse?.Data?.Token != null) 
                    {
                        _V7client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userexistsDataResponse?.Data?.Token);
                        HttpResponseMessage response2 = await _V7client.PostAsync($"auth/changepassword", content2);
                        string responseData = await response2.Content.ReadAsStringAsync();
                        var changePasswordResponse = JsonConvert.DeserializeObject<ChangePasswordResponse>(responseData);
                        if (Convert.ToBoolean(changePasswordResponse?.Data))
                        {
                            TempData["InfoMessage"] = "Successfully Changed Password!";
                            return RedirectToAction("Index", "home");
                        }
                        return View();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Incorrect Old Password!";
                        return View();
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Incorrect Old Password!";
                    return View();
                }
            }
        }
    }
}
