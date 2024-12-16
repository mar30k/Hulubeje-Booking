using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.HotelModels;
using HulubejeBooking.Models.PaymentModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace HulubejeBooking.Controllers.HotelController
{
    public class HotelController : Controller
    {
        private readonly ILogger<HotelController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationManager _authenticationManager;
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public HotelController(ILogger<HotelController> logger,
            IHttpClientFactory httpClientFactory, AuthenticationManager authenticationManager,
             IHttpContextAccessor? httpContextAccessor)
        {
            _authenticationManager = authenticationManager;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            var token = "";
            var identificationResult = await _authenticationManager.identificationValid();
            if (identificationResult != null)
            {
                token = identificationResult.UserData.Token;
                ViewBag.isVaild = identificationResult.isValid;
                ViewBag.isLoggedIn = identificationResult.isLoggedIn;
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
            var getcities = new GetCities();
            var getcompaniesbytyp = new GetcompaniesbyType();
            _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage getcitiesResponse = await _v7Client.GetAsync("getcities");
            HttpResponseMessage getcompaniesbytypeResponse = await _v7Client.GetAsync("routing/getcompaniesbytype?industryType=1989");
            if (getcitiesResponse.IsSuccessStatusCode && getcompaniesbytypeResponse.IsSuccessStatusCode)
            {
                string getcitiesData = await getcitiesResponse.Content.ReadAsStringAsync();
                string getcompaniesbytypeData = await getcompaniesbytypeResponse.Content.ReadAsStringAsync();
                getcompaniesbytyp = getcompaniesbytypeData != null ? JsonConvert.DeserializeObject<GetcompaniesbyType>(getcompaniesbytypeData) : new GetcompaniesbyType();
                getcities = getcitiesData != null ? JsonConvert.DeserializeObject<GetCities>(getcitiesData) : new GetCities();
                if (getcitiesData != null)
                {
                    HttpContext.Session.SetString("GetCities", getcitiesData);
                }
            }
            
            var viewModel = new HotelHome
            {
                Cities = getcities,
                Companies = getcompaniesbytyp,
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Availability([FromBody] RoomFormData roomFormData)
        {
            if (roomFormData != null) {
                var b = await _authenticationManager.identificationValid();
                string? token = b.UserData.Token;
                var dt = DateRangeParser.ParseDateRange(roomFormData.Date);
                var arrivalDateString = dt.startDateString.Trim();
                var departureDateString = dt.endDateString.Trim();
                DateTime arrivalDate = DateTime.ParseExact(arrivalDateString, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                DateTime departureDate = DateTime.ParseExact(departureDateString, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                roomFormData.ArrivalDate = arrivalDate;
                roomFormData.DepartureDate = departureDate;
                var roomFormDatajson = JsonConvert.SerializeObject(roomFormData);
                HttpContext.Session.SetString("RoomFormData", roomFormDatajson);
                var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
                var companyCode = roomFormData.orgCode;
                var orgOUD = roomFormData.oud;
                var adultCount = roomFormData.adultCount;
                var childCount = roomFormData.childrenCount;
                var roomCount = roomFormData.roomsCount;
                var requestBody = new
                {
                    arrivalDate,
                    departureDate,
                    companyCode,
                    adultCount,
                    childCount,
                    roomCount,
                    orgOUD,
                    city = (string)null
                };
                var roomBody = JsonConvert.SerializeObject(requestBody);
                var roomContent = new StringContent(roomBody, Encoding.UTF8, "application/json");

                _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _v7Client.PostAsync("hotel/getrooms", roomContent);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    var availableRooms = data != null ? JsonConvert.DeserializeObject<GetRooms>(data) : new GetRooms();
                    if (availableRooms != null)
                    {
                        availableRooms.HotelName = roomFormData.Name;
                    }
                    var availableRoomsString = JsonConvert.SerializeObject(availableRooms);
                    if (data != null)
                    {
                        HttpContext.Session.SetString("AvailabilityViewModel", availableRoomsString);
                    }
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            else 
            { 
                return BadRequest();
            }

    }
}
