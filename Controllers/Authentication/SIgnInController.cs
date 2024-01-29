using HulubejeBooking.Models.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace HulubejeBooking.Controllers.Authentication
{
    public class SignInController : Controller
    {
        private readonly ILogger<SignInController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public SignInController(ILogger<SignInController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AuthenticateUser(LoginInformation data)
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
                return RedirectToAction("Index", "Home");
            }
            else {
                ModelState.AddModelError("", "Incorrect Username or Password!");
                return RedirectToAction("Index", "SignIn");
            }
        }
    }
}