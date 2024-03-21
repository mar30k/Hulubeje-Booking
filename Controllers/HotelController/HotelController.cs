using HulubejeBooking.Models.HotelModels;
using HulubejeBooking.Controllers.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using HulubejeBooking.Models.Authentication;

namespace HulubejeBooking.Controllers.HotelController
{
    public class HotelController : Controller
    {
        private readonly ILogger<HotelController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HotelListBuffer _hotelListBuffer;
        private readonly AuthenticationManager _authenticationManager;
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public HotelController(ILogger<HotelController> logger,
            IHttpClientFactory httpClientFactory, AuthenticationManager authenticationManager,
             HotelListBuffer hotelListBuffer, IHttpContextAccessor? httpContextAccessor)
        {
            _authenticationManager = authenticationManager;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _hotelListBuffer = hotelListBuffer;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            var token = "";
            var userDataCookie = _httpContextAccessor?.HttpContext?.Request.Cookies[CNET_WebConstants.IdentificationCookie];
            string? password = "";
            string? code = "";
            if (!string.IsNullOrEmpty(userDataCookie))
            {
                var user = JsonConvert.DeserializeObject<UserInformation>(userDataCookie);
                password = user != null ? user?.password : "";
                code = user != null ? user?.phoneNumber : "";
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
            var identificationResult = await _authenticationManager.identificationValid();
            ViewBag.isVaild = identificationResult.isValid;
            ViewBag.isLoggedIn = identificationResult.isLoggedIn;
            if (identificationResult.isValid || identificationResult.isLoggedIn)
            {
                var param = new
                {
                    code,
                    password,
                    isChangePassword = false
                };
                var jsonRequest = JsonConvert.SerializeObject(param);
                
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage loginResponse = await _v7Client.PostAsync("auth/login", content);
                if (loginResponse.IsSuccessStatusCode)
                {
                    string loginData = await loginResponse.Content.ReadAsStringAsync();
                    var loginresponseData = JsonConvert.DeserializeObject<LoginAuthentication>(loginData);
                    if (loginresponseData != null && loginresponseData.IsSuccessful == true)
                    {
                        token = loginresponseData?.Data?.Token;
                        if (token != null) { HttpContext.Session.SetString("loginToken", token); }
                }

                
            }
            }
            else
            {
                var param = new
                {
                    code = "0000000000",
                    password = "0000000000",
                    isChangePassword = false
                };
                var jsonRequest = JsonConvert.SerializeObject(param);

                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage loginResponse = await _v7Client.PostAsync("auth/login", content);
                if (loginResponse.IsSuccessStatusCode)
            {
                    var loginData = await loginResponse.Content.ReadAsStringAsync();
                    var loginresponseData = JsonConvert.DeserializeObject<LoginAuthentication>(loginData);
                    if (loginresponseData != null && loginresponseData.IsSuccessful == true)
                                {
                        token = loginresponseData?.Data?.Token;
                        if (token != null) { HttpContext.Session.SetString("loginToken", token); }
                                }
                }
            }
            var getcities = new GetCities();
            var getcompaniesbytyp = new GetcompaniesbyType();
            _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage getcitiesResponse = await _v7Client.GetAsync("getcities");
            HttpResponseMessage getcompaniesbytypeResponse = await _v7Client.GetAsync("routing/getcompaniesbytype?industryType=1989");
            if (getcitiesResponse.IsSuccessStatusCode && getcitiesResponse.IsSuccessStatusCode) { 
                string getcitiesData = await getcitiesResponse.Content.ReadAsStringAsync();
                string getcompaniesbytypeData = await getcompaniesbytypeResponse.Content.ReadAsStringAsync();
                getcompaniesbytyp = getcompaniesbytypeData!=null? JsonConvert.DeserializeObject<GetcompaniesbyType>(getcompaniesbytypeData) : new GetcompaniesbyType();   
                getcities = getcitiesData!=null ?JsonConvert.DeserializeObject<GetCities>(getcitiesData) : new GetCities();   
            }
            var viewModel = new HotelHome
                        {
                Cities = getcities,
                Companies = getcompaniesbytyp,
                        };
            }
            




            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> HotelList(string city, int roomsCount, int numberOfNights,int childrenCount,int adultCount, string dateRange)
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
            var b = await _authenticationManager.identificationValid();
            ViewBag.isVaild = b.isValid;
            ViewBag.isLoggedIn = b.isLoggedIn;
            var _client = _httpClientFactory.CreateClient("CnetHulubeje");
            var dataList = new List<GetModel>();

            var cityName = city;
            var adultsCount = adultCount;
            var childCount = childrenCount;
            var roomCount = roomsCount;
            var dt = DateRangeParser.parseDateRange(dateRange);
            var arrivalDate = dt.startDateString;
            var departureDate = dt.endDateString;
            var numberOfDay = numberOfNights;

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
                            .Where(branch => branch != null && branch.City != null && branch.City.Equals(cityName, StringComparison.OrdinalIgnoreCase))
                            .Select(branch =>
                                new FilteredCompany
                                {
                                    isSponsored = hotel.IsSponsored,
                                    code = hotel.Code,
                                    tradeName = hotel.TradeName,
                                    brandName = hotel.BrandName ?? hotel.TradeName,
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
                    adultCount = adultsCount,
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

                var hotels = new List<HotelModel>();

                HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "/Industry/GetHotelsFilteredByCity", content);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    hotels = data != null ? JsonConvert.DeserializeObject<List<HotelModel>>(data) : new List<HotelModel>();
                }

                var viewModel = new HotelViewModel
                {
                    Hotels = hotels,
                    Rooms = new List<RoomModel>()
                };
                var viewModelJson = JsonConvert.SerializeObject(viewModel);

                HttpContext.Session.SetString("HotelViewModel", viewModelJson);
                return View(viewModel);

                
                

            }
            else
            {
                return Ok();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Availability([FromBody] RoomFormData roomFormData)
        {
            var _client = _httpClientFactory.CreateClient("CnetHulubeje");
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
                var availableRooms = data != null ? JsonConvert.DeserializeObject<List<RoomModel>>(data) : new List<RoomModel>();


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

        public async Task<IActionResult> Hoteldetail([FromBody] RoomFormData roomFormData)
        {
            try
            {
                var _client = _httpClientFactory.CreateClient("CnetHulubeje");

                var orgTin = roomFormData.orgTin;
                var oud = roomFormData.oud;

                HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + $"/Industry/GetCompanyBranchDetail?orgTin={orgTin}&branchCode={oud}&industryType=LKUP000000451");
                HttpResponseMessage imageResponse = await _client.GetAsync(_client.BaseAddress + $"/Ecommerce/GetSlidingImagesForSingleCompany?orgTin={orgTin}&oud={oud}");
                if (response.IsSuccessStatusCode )
                {
                    var images = new List<string>();
                    if (imageResponse.IsSuccessStatusCode) 
                    {
                        string image = await imageResponse.Content.ReadAsStringAsync();
                        images = JsonConvert.DeserializeObject<List<string>>(image);
                    }
                    string content = await response.Content.ReadAsStringAsync();
                    var hotel = JsonConvert.DeserializeObject<HotelDetailModel>(content);
                    if (hotel != null )
                    {
                        hotel.Images = images?.Count > 0 ? images : new List<string>();
                        hotel.Name = roomFormData.Name;
                    }
                    var viewModelJson = JsonConvert.SerializeObject(hotel);

                    HttpContext.Session.SetString("hotelDetail", viewModelJson);


                    return Json(new HotelDetailModel());
                }
                else
                {
                    
                    return View(); // Replace "ErrorView" with the name of your error view
                }
            }
            catch (Exception ex)
            {
               return View("ErrorView");
            }
        }

        public async Task<IActionResult> HoteldetailViewAsync()
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
            var b = await _authenticationManager.identificationValid();
            ViewBag.isVaild = b.isValid;
            ViewBag.isLoggedIn = b.isLoggedIn;
            var value = HttpContext.Session.GetString("hotelDetail");
            if (!string.IsNullOrEmpty(value))
            {
                HttpContext.Session.Remove("HotelViewModel");

                var viewModel = JsonConvert.DeserializeObject<HotelDetailModel>(value);

                return View(viewModel);
            }
            else
            {
                return View("Error View");
            }
        }



    }
}
