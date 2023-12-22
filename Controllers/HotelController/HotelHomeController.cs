using HulubejeBooking.Models.HotelModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
    }
}
