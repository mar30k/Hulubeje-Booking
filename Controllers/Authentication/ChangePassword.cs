using Microsoft.AspNetCore.Mvc;
using HulubejeBooking.Models.Authentication;
using Newtonsoft.Json;
using System.Text;
namespace HulubejeBooking.Controllers.Authentication
{
    public class ChangePassword : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;
        public ChangePassword(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            var userDataCookie = _httpContextAccessor?.HttpContext?.Request.Cookies[CNET_WebConstants.IdentificationCookie];
            if (!string.IsNullOrEmpty(userDataCookie))
            {
                var user = JsonConvert.DeserializeObject<UserInformation>(userDataCookie);
                ViewBag.FirstName = user?.firstName;
                ViewBag.LastName = user?.lastName;
                ViewBag.MiddleName = user?.middleName;
                ViewBag.Personalattachment = user?.personalattachment;
                ViewBag.SuccessCode = user?.successCode;
                ViewBag.Idnumber = user?.idnumber;
                ViewBag.Idtype = user?.idtype;
                ViewBag.Dob = user?.dob;
                ViewBag.Idattachment = user?.idattachment;
                ViewBag.PhoneNumber = user?.phoneNumber;
                ViewBag.EmailAddress = user?.emailAddress;
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(ChangePasswordModel changePassword)
        {
            var _client = _httpClientFactory.CreateClient("CnetHulubeje");
            var userDataCookie = _httpContextAccessor?.HttpContext?.Request.Cookies[CNET_WebConstants.IdentificationCookie];
            string? phoneNumber = "";
            if (!string.IsNullOrEmpty(userDataCookie))
            {
                var user = JsonConvert.DeserializeObject<UserInformation>(userDataCookie);
                phoneNumber = user != null ? user?.phoneNumber : null;
            }
            if (changePassword?.Newpassword?.Length < 6 || changePassword?.RepeatPassword?.Length < 6)
            {
                TempData["ErrorMessage"] = "password lenght should be atleast 6 characters ";
                return View();
            }
            else if (changePassword?.RepeatPassword != changePassword?.Newpassword)
            {
                TempData["ErrorMessage"] = "new passwords should match!";
                return View();
            }
            else if (changePassword?.RepeatPassword?.Length < 6)
            {
                TempData["ErrorMessage"] = "incorrect current password";
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
                    password=changePassword?.RepeatPassword,
                };
                var param2 = new
                {
                    username = phoneNumber,
                    password = changePassword?.Oldpassword,
                };
                string jsonBody = JsonConvert.SerializeObject(param2);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "/Profile/authenticateUser", content);
                string responseData = await response.Content.ReadAsStringAsync();
                var userInformationResponse = JsonConvert.DeserializeObject<List<UserResponse>>(responseData);
                var userInformation = userInformationResponse?[0].userInformation;
                if (userInformation != null) {
                    string jsonBody2 = JsonConvert.SerializeObject(param);
                    var content2 = new StringContent(jsonBody2, Encoding.UTF8, "application/json");
                    HttpResponseMessage response2 = await _client.PostAsync(_client.BaseAddress + $"/Profile/updatePassword", content2);
                    string responseData2 = await response2.Content.ReadAsStringAsync();
                    if (Convert.ToBoolean(responseData2))
                    {
                        TempData["InfoMessage"] = "Successfully Changed Password!";
                        return RedirectToAction("Index", "home");
                    }
                    return View();
                }
                else
                {
                    TempData["ErrorMessage"] = "incorrect current password";
                    return View();
                }
            }
        }
    }
}
