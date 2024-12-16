using HulubejeBooking.Models.HotelModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using HulubejeBooking.Controllers.Authentication;

namespace HulubejeBooking.Controllers.HotelController
{
    public class HotelListController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationManager _authenticationManager;

        public HotelListController(IHttpClientFactory httpClientFactory, AuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Index(int city, int roomsCount, int numberOfNights, int childrenCount, int adultCount, string dateRange)
        {
            var citiesString = HttpContext.Session.GetString("GetCities");
            var citiesJson = citiesString != null ? JsonConvert.DeserializeObject<GetCities>(citiesString) : new GetCities();
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            var gethotelsbycity = new GetHotelByCity();
            var identificationResult = await _authenticationManager.identificationValid();
            if (identificationResult != null)
            {
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
            string? token = identificationResult?.UserData.Token;
            var childCount = childrenCount;
            var roomCount = roomsCount;
            var dt = DateRangeParser.ParseDateRange(dateRange);
            var arrivalDateString = dt.startDateString.Trim();
            var departureDateString = dt.endDateString.Trim();
            DateTime arrivalDate = DateTime.ParseExact(arrivalDateString, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime departureDate = DateTime.ParseExact(departureDateString, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            var data = new RoomFormData
            {
                city = city,
                roomsCount = roomsCount,
                childrenCount = childrenCount,
                adultCount = adultCount,
                Date = dateRange,
                numberOfNights = numberOfNights,
                DepartureDate = departureDate,
                ArrivalDate = arrivalDate,
            };
            var numberOfDay = numberOfNights;

            if (numberOfDay <= 0)
            {
                numberOfDay = 1;
            }
            _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var param = new
            {
                companyCode = (string?)null,
                orgOUD = (string?)null,
                arrivalDate,
                departureDate,
                adultCount,
                childCount,
                roomCount,
                city,
                page = 1
            };
            var paramJson = JsonConvert.SerializeObject(param);
            var content = new StringContent(paramJson, Encoding.UTF8, "application/json");

            HttpResponseMessage gethotelsbycityResponse = await _v7Client.PostAsync($"hotel/gethotelsbycity", content);
            if (gethotelsbycityResponse.IsSuccessStatusCode)
            {
                string gethotelsbycityData = await gethotelsbycityResponse.Content.ReadAsStringAsync();
                gethotelsbycity = gethotelsbycityData != null ? JsonConvert.DeserializeObject<GetHotelByCity>(gethotelsbycityData) : new GetHotelByCity();
                if (gethotelsbycity != null && citiesJson != null)
                {
                    var cityData = citiesJson?.Data?.FirstOrDefault(c => c.Id == city);
                    int hotelsCount = (int)(cityData?.Hotels != null ? cityData.Hotels : 0);
                    gethotelsbycity.HotelsCount = (int)Math.Ceiling((double)hotelsCount / 10);
                    gethotelsbycity.RoomFormData = data;
                }

            }
            return View(gethotelsbycity);
        }
        [Route("AdditionalPages")]
        [HttpGet]
        public async Task<IActionResult> AdditionalPages(int city, int roomsCount, int numberOfNights, int childrenCount, int adultCount, string dateRange, int page)
        {
            var numberOfDay = numberOfNights <= 0 ? 1 : numberOfNights;
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            var gethotelsbycity = new GetHotelByCity();
            var identificationResult = await _authenticationManager.identificationValid();
            string? token = identificationResult?.UserData.Token;
            var dt = DateRangeParser.ParseDateRange(dateRange);
            var arrivalDateString = dt.startDateString.Trim();
            var departureDateString = dt.endDateString.Trim();
            DateTime arrivalDate = DateTime.ParseExact(arrivalDateString, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime departureDate = DateTime.ParseExact(departureDateString, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            var data = new RoomFormData
            {
                city = city,
                roomsCount = roomsCount,
                childrenCount = childrenCount,
                adultCount = adultCount,
                Date = dateRange,
                numberOfNights = numberOfDay,
                DepartureDate = departureDate,
                ArrivalDate = arrivalDate,
            };

            _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var param = new
            {
                companyCode = (string?)null,
                orgOUD = (string?)null,
                arrivalDate,
                departureDate,
                adultCount,
                childCount = childrenCount,
                roomCount = roomsCount,
                city,
                page
            };
            var paramJson = JsonConvert.SerializeObject(param);
            var content = new StringContent(paramJson, Encoding.UTF8, "application/json");

            HttpResponseMessage gethotelsbycityResponse = await _v7Client.PostAsync($"hotel/gethotelsbycity", content);
            if (gethotelsbycityResponse.IsSuccessStatusCode)
            {
                string gethotelsbycityData = await gethotelsbycityResponse.Content.ReadAsStringAsync();
                gethotelsbycity = gethotelsbycityData != null ? JsonConvert.DeserializeObject<GetHotelByCity>(gethotelsbycityData) : new GetHotelByCity();
                if (gethotelsbycity != null)
                {
                    gethotelsbycity.RoomFormData = data;
                }
            }
            return Json(gethotelsbycity);
        }

    }
}
