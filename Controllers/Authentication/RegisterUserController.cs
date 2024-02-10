using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.HotelModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace HulubejeBooking.Controllers.Authentication
{
    public class RegisterUserController : Controller
    {
        private readonly ILogger<RegisterUserController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationManager _authenticationManager;
        public RegisterUserController(ILogger<RegisterUserController> logger, IHttpClientFactory httpClientFactory, AuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Index()
        {
            var _client = _httpClientFactory.CreateClient("CnetHulubeje");
            var data = HttpContext.Session.GetString("UserInfo");
            var newData = new PersonModel();
            if (data != null) {
                newData = JsonConvert.DeserializeObject<PersonModel>(data);
            }
            var param = new
            {
                personCode = newData?.PhoneNumber,
                firstName = newData?.FirstName,
                lastName = newData?.LastName,
                middleName = newData?.MiddleName,
                emailAddress = newData?.EmailAddress,
                phoneNumber = newData?.PhoneNumber,
                gender = newData?.Gender,
                dob = newData?.Dob,
                password = newData?.Password,
                idType = newData?.IdType,
                id = newData?.Id,
                personalPhoto = newData?.PersonalPhoto,
                idPhoto = newData?.IdPhoto,
            };
            string jsonBody = JsonConvert.SerializeObject(param);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "/Profile/createUser", content);
            string responseData = await response.Content.ReadAsStringAsync();
            if (responseData != null && Convert.ToBoolean(responseData))
            {
                var isLoggedIn = true;
                string isLoggedInJson = JsonConvert.SerializeObject(isLoggedIn);
                HttpContext.Session.SetString("isLoggedIn", isLoggedInJson);

                var userInformation = new UserInformation()
                {
                    code = newData?.PhoneNumber,
                };
                _authenticationManager.SignIn(userInformation, true);
                TempData["InfoMessage"] = "Welcome! You Have Successfully Created Hulubeje Account";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ErrorMessage"] = "It seems like this phone number is already registered. Please use a different number or sign in.";
                return RedirectToAction("Index", "Home");
            }

        }
    }
}
