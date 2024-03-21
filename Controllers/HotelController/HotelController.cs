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
        public async Task<IActionResult> HotelList(int city, int roomsCount, int numberOfNights, int childrenCount, int adultCount, string dateRange)
        {
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            var token = "";
            var gethotelsbycity = new GetHotelByCity();
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
            var arrivalDateString = dt.startDateString.Trim();
            var departureDateString = dt.endDateString.Trim();
            DateTime arrivalDate = DateTime.ParseExact(arrivalDateString, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime departureDate = DateTime.ParseExact(departureDateString, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);

            var numberOfDay = numberOfNights;

            if (numberOfDay <= 0)
            {
                numberOfDay = 1;
            }
            if (HttpContext.Session.TryGetValue("loginToken", out var loginToken))
            {
                token = Encoding.UTF8.GetString(loginToken);
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
                    city
                };
                var paramJson = JsonConvert.SerializeObject(param);
                var content = new StringContent(paramJson, Encoding.UTF8, "application/json");

                HttpResponseMessage gethotelsbycityResponse = await _v7Client.PostAsync($"hotel/gethotelsbycity", content);
                if (gethotelsbycityResponse.IsSuccessStatusCode)
                {
                    string gethotelsbycityData = await gethotelsbycityResponse.Content.ReadAsStringAsync();
                    gethotelsbycity = JsonConvert.DeserializeObject<GetHotelByCity>(gethotelsbycityData);
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
            return View(gethotelsbycity);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Availability([FromBody] RoomFormData roomFormData)
        {
			var token = "";
			var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            var dt = DateRangeParser.parseDateRange(roomFormData.Date);
            var arrivalDateString = dt.startDateString.Trim();
            var departureDateString = dt.endDateString.Trim();
            DateTime arrivalDate = DateTime.ParseExact(arrivalDateString, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DateTime departureDate = DateTime.ParseExact(departureDateString, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);


            var companyCode = roomFormData.orgCode;
            var orgOUD = roomFormData.oud;
            var adultCount = roomFormData.adultCount;
            var childCount = roomFormData.childrenCount;
            var roomCount = roomFormData.roomsCount;


            var requestBody = new
            {
                companyCode,
                arrivalDate,
                departureDate,
                adultCount,
                childCount,
                roomCount,
                orgOUD,
                city =(string)null
            };
            var roomBody = JsonConvert.SerializeObject(requestBody);
            var roomContent = new StringContent(roomBody, Encoding.UTF8, "application/json");
            if (HttpContext.Session.TryGetValue("loginToken", out var loginToken))
            {
                token = Encoding.UTF8.GetString(loginToken);
                _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _v7Client.PostAsync("hotel/getrooms", roomContent);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
					var availableRooms = data != null ? JsonConvert.DeserializeObject<GetRooms>(data) : new GetRooms();
                    var availableRoomsJson = JsonConvert.SerializeObject(availableRooms);
                    HttpContext.Session.SetString("AvailabilityViewModel", data);
					return Ok();    
				}
				else
                {
					return BadRequest();
				}
            }
            else
            {
				TempData["ErrorMessage"] = "Session Has Expired Please Restart the Booking Process";
				return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Hoteldetail([FromBody] RoomFormData roomFormData)
        {
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            var token = "";
            try
            {
                if (HttpContext.Session.TryGetValue("loginToken", out var loginToken))
                {
                    token = Encoding.UTF8.GetString(loginToken);
                    _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    HttpResponseMessage getcompanyscheduleResponse = await _v7Client.GetAsync($"routing/getcompanyschedule?companyCode={roomFormData.orgTin}&branchCode={roomFormData.oud}&industryType=1989");
                    var getsupplierpaymentoptions = new PaymentProcessorResponse();
                    var getcompanyschedule = new GetCompanySchedule();
                    var getcompanyimages = new GetCompanyImages();
                    if (getcompanyscheduleResponse.IsSuccessStatusCode)
                    {
                        string getcompanyscheduleData =await getcompanyscheduleResponse.Content.ReadAsStringAsync();
                        getcompanyschedule = JsonConvert.DeserializeObject<GetCompanySchedule>(getcompanyscheduleData);
                    }

                    HttpResponseMessage getsupplierpaymentoptionsResponse = await _v7Client.GetAsync($"payment/getsupplierpaymentoptions?code={roomFormData.orgCode}&branchCode={roomFormData.oud}");
                    HttpResponseMessage getcompanyimagesResponse = await _v7Client.GetAsync($"routing/getcompanyimages?tin={roomFormData.orgTin}&branchCode={roomFormData.oud}&industryType=1989");

                    if (getsupplierpaymentoptionsResponse.IsSuccessStatusCode)
                {
                        string getsupplierpaymentoptionsData = await getsupplierpaymentoptionsResponse.Content.ReadAsStringAsync();
                        getsupplierpaymentoptions  = JsonConvert.DeserializeObject<PaymentProcessorResponse>(getsupplierpaymentoptionsData);
                    }
                    if (getcompanyimagesResponse.IsSuccessStatusCode)
                    {
                        string getcompanyimagesData = await getcompanyimagesResponse.Content.ReadAsStringAsync();
                        getcompanyimages = JsonConvert.DeserializeObject<GetCompanyImages>(getcompanyimagesData);
                    }
                    var HotelDetailModel = new HotelDetailModel
                    {
                        PaymentOptions = getsupplierpaymentoptions,
                        Description = roomFormData.Description,
                        Name = roomFormData.Name,
                        CompanySchedule = getcompanyschedule,
						CompanyCode = roomFormData.orgCode,
						OrgOUD = roomFormData.oud,
                        ImageModel = getcompanyimages
                    };
                    var HotelDetailJson = JsonConvert.SerializeObject(HotelDetailModel);
                    HttpContext.Session.SetString("HotelDetailModel", HotelDetailJson);
                    return Ok();
                }
                else
                {
                    TempData["ErrorMessage"] = "Session Has Expired Please Restart the Booking Process";
                    return RedirectToAction("Index", "Home");
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
            var value = HttpContext.Session.GetString("HotelDetailModel");
            if (!string.IsNullOrEmpty(value))
            {
                //HttpContext.Session.Remove("HotelViewModel");

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
