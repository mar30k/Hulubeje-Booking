using HulubejeBooking.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace HulubejeBooking.Controllers.Authentication
{
    public class SignInController : Controller
    {
        private readonly ILogger<SignInController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly AuthenticationManager _authenticationManager;
        public SignInController(ILogger<SignInController> logger, IHttpClientFactory httpClientFactory,
            AuthenticationManager authenticationManager)
        {
            _logger = logger;
            _authenticationManager = authenticationManager;
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(LoginInformation data)
        { 
            var _client = _httpClientFactory.CreateClient("CnetHulubeje");
            var param = new
            {
                username = data.Phone,
                password = data.Password,
            };
            string jsonBody = JsonConvert.SerializeObject(param);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "/Profile/authenticateUser", content);
            string responseData = await response.Content.ReadAsStringAsync();
            var userInformationResponse = JsonConvert.DeserializeObject<List<UserResponse>>(responseData);
            var userInformation = userInformationResponse?[0].userInformation;
            if (userInformation != null)
            {
                _authenticationManager.SignIn(userInformation, data.RememberMe);
                var signInInformation = HttpContext.Session.GetString("SignInInformation");
                var signin = "";
                var isLoggedIn = true;
                string isLoggedInJson = JsonConvert.SerializeObject(isLoggedIn);

                HttpContext.Session.SetString("isLoggedIn", isLoggedInJson);

                if (signInInformation != null)
                {
                    signin = JsonConvert.DeserializeObject<string>(signInInformation);
                }
                HttpContext.Session.Remove("SignInInformation");

                if (signin == "Hotel")
                {
                    string validation = "Yes";
                    var validationJson = JsonConvert.SerializeObject(validation);
                    HttpContext.Session.SetString("IsLogin", validationJson);
                    return RedirectToAction("Index", "FilteredRoom");
                }

                else if (signin == "Cinema")
                {
                    string validation = "Yes";
                    var validationJson = JsonConvert.SerializeObject(validation);
                    HttpContext.Session.SetString("IsLogin", validationJson);
                    return RedirectToAction("Index", "CinemaSeatLayout");
                }
                else if (signin == "Bus")
                {
                    string validation = "Yes";
                    var validationJson = JsonConvert.SerializeObject(validation);
                    HttpContext.Session.SetString("IsLogin", validationJson);
                    return RedirectToAction("Index", "BusSeatLayout");
                }
                else 
                {
                    TempData["InfoMessage"] = "Welcome! You have successfully logged in.";
                    return RedirectToAction("Index", "Home");
                }
            }
            else {
                ModelState.AddModelError("", "Incorrect Username or Password!");
                return View("Index");
            }
        }
        public IActionResult Logout()
        {
            var isLoggedIn = false;
            string isLoggedInJson = JsonConvert.SerializeObject(isLoggedIn);
            HttpContext.Session.SetString("isLoggedIn", isLoggedInJson);
            _authenticationManager.SignOut();
            TempData["InfoMessage"] = "You have successfully logged Out.";
            return RedirectToAction("Index", "Home");
        }
    }
}