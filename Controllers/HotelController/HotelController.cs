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
        [Route("hotellist")]
        [HttpGet]
        public async Task<IActionResult> HotelList(int city, int roomsCount, int numberOfNights, int childrenCount, int adultCount, string dateRange)
        {
            var citiesString = HttpContext.Session.GetString("GetCities");
            var citiesJson = citiesString!=null ?JsonConvert.DeserializeObject<GetCities>(citiesString) : new GetCities ();
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
                companyCode = (string)null,
                orgOUD = (string)null,
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
                if (gethotelsbycity != null && citiesJson!=null)
                {
                    var cityData = citiesJson?.Data?.FirstOrDefault(c => c.Id == city);
                    int hotelsCount = (int)(cityData?.Hotels != null ? cityData.Hotels : 0);
                    gethotelsbycity.HotelsCount = (int)Math.Ceiling((double)hotelsCount / 10);
                    gethotelsbycity.RoomFormData = data;
                }

            }
            return View(gethotelsbycity);
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

        public async Task<IActionResult> Hoteldetail([FromBody] RoomFormData roomFormData)
        {
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            var b = await _authenticationManager.identificationValid();
            string? token = b.UserData.Token;
            try
            {
                _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage getcompanyscheduleResponse = await _v7Client.GetAsync($"routing/getcompanyschedule?companyCode={roomFormData.orgCode}&branchCode={roomFormData.oud}&industryType=1989");
                var getsupplierpaymentoptions = new PaymentProcessorResponse();
                var getcompanyschedule = new GetCompanySchedule();
                var getcompanyimages = new GetCompanyImages();
                if (getcompanyscheduleResponse.IsSuccessStatusCode)
                {
                    string getcompanyscheduleData = await getcompanyscheduleResponse.Content.ReadAsStringAsync();
                    getcompanyschedule = JsonConvert.DeserializeObject<GetCompanySchedule>(getcompanyscheduleData);
                }

                HttpResponseMessage getsupplierpaymentoptionsResponse = await _v7Client.GetAsync($"payment/getsupplierpaymentoptions?code={roomFormData.orgCode}&branchCode={roomFormData.oud}");
                HttpResponseMessage getcompanyimagesResponse = await _v7Client.GetAsync($"routing/getcompanyimages?tin={roomFormData.orgTin}&branchCode={roomFormData.oud}&industryType=1989");

                if (getsupplierpaymentoptionsResponse.IsSuccessStatusCode)
                {
                    string getsupplierpaymentoptionsData = await getsupplierpaymentoptionsResponse.Content.ReadAsStringAsync();
                    getsupplierpaymentoptions = JsonConvert.DeserializeObject<PaymentProcessorResponse>(getsupplierpaymentoptionsData);
                }
                if (getcompanyimagesResponse.IsSuccessStatusCode)
                {
                    string getcompanyimagesData = await getcompanyimagesResponse.Content.ReadAsStringAsync();
                    getcompanyimages = JsonConvert.DeserializeObject<GetCompanyImages>(getcompanyimagesData);
                }
                var CompanyDetailModel = new CompanyDetailModel
                {
                    BranchName = roomFormData.BranchName,
                    PaymentOptions = getsupplierpaymentoptions,
                    Description = roomFormData.Description,
                    Name = roomFormData.Name,
                    CompanySchedule = getcompanyschedule,
                    CompanyCode = roomFormData.orgCode,
                    OrgOUD = roomFormData.oud,
                    CityName = roomFormData.CityName,
                    ImageModel = getcompanyimages
                };
                var HotelDetailJson = JsonConvert.SerializeObject(CompanyDetailModel);
                HttpContext.Session.SetString("CompanyDetailModel", HotelDetailJson);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("hoteldetail")]
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
            var value = HttpContext.Session.GetString("CompanyDetailModel");
            if (!string.IsNullOrEmpty(value))
            {
                //HttpContext.Session.Remove("HotelViewModel");

                var viewModel = JsonConvert.DeserializeObject<CompanyDetailModel>(value);

                return View(viewModel);
            }
            else
            {
                TempData["ErrorMessage"] = "Session Has Expired Please Restart the Booking Process";
                return RedirectToAction("Index", "Home");
            }
        }

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
