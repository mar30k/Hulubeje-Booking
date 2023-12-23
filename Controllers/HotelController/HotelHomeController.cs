using HulubejeBooking.Models.HotelModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace HulubejeBooking.Controllers.HotelController
{
    public class HotelHomeController : Controller
    {
        private readonly ILogger<HotelHomeController> _logger;
        private readonly object JsonRequestBehavior;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HotelListBuffer _hotelListBuffer;

        public HotelHomeController(ILogger<HotelHomeController> logger,
            IHttpClientFactory httpClientFactory,
             HotelListBuffer hotelListBuffer)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _hotelListBuffer = hotelListBuffer;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var _client = _httpClientFactory.CreateClient("HotelBooking");


            HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + "/Industry/GetAllCompanies?industryType=LKUP000000451");

            List<GetModel> hotels = new List<GetModel>();
            List<CityData> country = new List<CityData>();
            HotelListRequest viewModel = new HotelListRequest();

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                hotels = JsonConvert.DeserializeObject<List<GetModel>>(data);


                HttpResponseMessage response2 = await _client.GetAsync(_client.BaseAddress + "/Industry/GetCitiesForHotelFilter");
                if (response2.IsSuccessStatusCode)
                {
                    string data2 = await response2.Content.ReadAsStringAsync();
                    country = JsonConvert.DeserializeObject<List<CityData>>(data2);
                }


                viewModel = new HotelListRequest
                {
                    HotelList = hotels,
                    CityNameList = country
                };
            }

            _hotelListBuffer._hotels = hotels;

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> HotelList([FromBody] FormData formData)
        {

            var _client = _httpClientFactory.CreateClient("HotelBooking");
            List<GetModel> dataList = new List<GetModel>();

            var cityName = formData.City;
            var adultCount = formData.AdultCount;
            var childCount = formData.ChildrenCount;
            var roomCount = formData.RoomsCount;
            var dt = DateRangeParser.parseDateRange(formData.DateRange);
            var arrivalDate = dt.startDateString;
            var departureDate = dt.endDateString;
            var numberOfDay = formData.numberOfNights;

            if (numberOfDay <= 0)
            {
                numberOfDay = 1;
            }


            dataList = _hotelListBuffer._hotels;
            if (dataList != null)
            {
                var filteredCompanies = dataList
                    .SelectMany(hotel =>
                        hotel.Branches
                            .Where(branch => branch.City.Equals(cityName, StringComparison.OrdinalIgnoreCase))
                            .Select(branch =>
                                new FilteredCompany
                                {
                                    isSponsored = hotel.IsSponsored,
                                    code = hotel.Code,
                                    tradeName = hotel.TradeName,
                                    brandName = hotel.BrandName,
                                    industryType = hotel.IndustryType,
                                    rating = hotel.Rating,
                                    TIN = hotel.TIN,
                                    attachments = hotel.Attachments,
                                    registerDate = hotel.RegisterDate,
                                    isTaxInclusive = hotel.IsTaxInclusive,
                                    termsAndConditionUrl = hotel.TermsAndConditionsUrl,
                                    ratingCount = (int)hotel.RatingCount,
                                    oud = branch.Code,
                                    branchName = branch.BranchName,
                                    branchCategory = branch.Category
                                }
                            )
                    )
                    .ToList();

                var filteredHotel = new FilteredHotel
                {
                    adultCount = adultCount,
                    childCount = childCount,
                    roomCount = roomCount,
                    numberOfDay = numberOfDay,
                    arrivalDate = arrivalDate,
                    departureDate = departureDate,
                    cityName = cityName,
                    filteredCompanies = filteredCompanies
                };


                string jsonBody = JsonConvert.SerializeObject(filteredHotel);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                List<HotelModel> hotels = new List<HotelModel>();

                HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "/Industry/GetHotelsFilteredByCity", content);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    hotels = JsonConvert.DeserializeObject<List<HotelModel>>(data);
                }

                var viewModel = new HotelViewModel
                {
                    Hotels = hotels,
                    Rooms = new List<RoomModel>()
                };
                var viewModelJson = JsonConvert.SerializeObject(viewModel);

                HttpContext.Session.SetString("HotelViewModel", viewModelJson);
                return Json(new HotelViewModel());

            }
            else
            {
                return Json(new HotelViewModel());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Availability([FromBody] RoomFormData roomFormData)
        {
            var _client = _httpClientFactory.CreateClient("HotelBooking");
            var dt = DateRangeParser.parseDateRange(roomFormData.Date);
            var arrivalDate = dt.startDateString;
            var departureDate = dt.endDateString;

            var orgTin = roomFormData.orgTin;
            var oud = roomFormData.oud;
            var adultCount = roomFormData.adultCount;
            var childCount = roomFormData.childrenCount;
            var roomCount = roomFormData.roomsCount;


            var requestBody = new
            {
                orgTin,
                arrivalDate,
                departureDate,
                adultCount,
                childCount,
                roomCount,
                oud
            };
            var roomBody = JsonConvert.SerializeObject(requestBody);
            var roomContent = new StringContent(roomBody, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(_client.BaseAddress + "/Hotel/getOnlineRoom", roomContent);


            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var availableRooms = JsonConvert.DeserializeObject<List<RoomModel>>(data);


                var viewModel = new AvailabilityViewModel
                {
                    AvailableRooms = availableRooms
                };


                var viewModelJson = JsonConvert.SerializeObject(viewModel);

                HttpContext.Session.SetString("AvailabilityViewModel", viewModelJson);
                return Json(new AvailabilityViewModel());
            }
            else
            {
                return Json(new AvailabilityViewModel());
            }
        }
    }
}
