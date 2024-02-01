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
            var newData = JsonConvert.DeserializeObject<PersonModel>(data);

            var param = new
            {
                personCode = newData?.phoneNumber,
                firstName = newData?.firstName,
                lastName = newData?.lastName,
                middleName = newData?.middleName,
                emailAddress = newData?.emailAddress,
                phoneNumber = newData?.phoneNumber,
                gender = newData?.gender,
                dob = newData?.dob,
                password = newData?.password,
                idType = newData?.idType,
                id = newData?.id,
                personalPhoto = newData?.personalPhoto,
                idPhoto = newData?.idPhoto,
            };
            string jsonBody = JsonConvert.SerializeObject(param);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "/Profile/createUser", content);
            string responseData = await response.Content.ReadAsStringAsync();
            var isLoggedIn = true;
            string isLoggedInJson = JsonConvert.SerializeObject(isLoggedIn);
            HttpContext.Session.SetString("isLoggedIn", isLoggedInJson);

            var userInformation = new UserInformation()
            {
                code = newData?.phoneNumber,
            };
            _authenticationManager.SignIn(userInformation, true);

            return RedirectToAction("Index", "Home");
        }
    }
}
