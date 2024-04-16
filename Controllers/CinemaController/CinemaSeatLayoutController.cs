using HulubejeBooking.Models.CInemaModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HulubejeBooking.Controllers.Authentication;
namespace CinemaSeatBooking.Controllers;
using HulubejeBooking.Controllers;

using HulubejeBooking.Models.Authentication;
using NuGet.Common;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Core.Extensions;

public class CinemaSeatLayoutController : Controller
{
    private readonly AuthenticationManager _authenticationManager;
    private readonly IHttpContextAccessor? _httpContextAccessor;
    private readonly IHttpClientFactory _httpClientFactory;

    public CinemaSeatLayoutController(AuthenticationManager authenticationManager, IHttpContextAccessor? httpContextAccessor, IHttpClientFactory httpClientFactory)
    {
        _authenticationManager = authenticationManager;
        _httpContextAccessor = httpContextAccessor;
        _httpClientFactory = httpClientFactory;
    }
    public async Task<IActionResult> IndexAsync(string? spacecode, string? companyTinNumber, string? branchCode, string? companyName,
        string? movieName, string? movieCode, string? dimension, string? spaceType, string? selectedDate, string? code,  string? hallName, string? utcTime,
        int speceId, string? id, string? companyCode, string? schdetailId, decimal? priceValue, string? articleCode)
    {
        int? companyscode = 0;
        if (HttpContext.Session.TryGetValue("movies", out var movies))
        {
            var moviesString = Encoding.UTF8.GetString(movies);
            var movie = JsonConvert.DeserializeObject<Movie>(moviesString);
            var companyData = movie?.Data?.FirstOrDefault(c => c.BranchCode.ToString() == branchCode);
            companyTinNumber = companyData?.TIN;
            companyscode = companyData?.CompanyCode;
            companyName = companyData?.CompanyName;
        }
        else
        {
            TempData["ErrorMessage"] = "Session Has Expired Please Restart the Booking Process";
            return RedirectToAction("Index", "Home");
        }
        string? phoneNumber = "";
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

        string? token = identificationResult?.UserData?.Token;
        phoneNumber = identificationResult?.UserData?.Code;

        var loginInfo = HttpContext.Session.GetString("IsLogin");
        var login = "";
        if (loginInfo != null)
        {
            login = JsonConvert.DeserializeObject<string>(loginInfo);
        }
        //HttpContext.Session.Remove("IsLogin");
        if (login != "Yes")
        {
            var param = new SeatLayout
            {
                Id = id,
                SpaceId = speceId,
                CompanyCode = companyCode,
                SpaceCode = spacecode,
                CompanyTinNumber = companyTinNumber,
                BranchCode = branchCode,
                CompanyName = companyName,
                MovieName = movieName,
                MovieCode = movieCode,
                Dimension = dimension,
                HallName = hallName,
                SpaceType = spaceType,
                SelectedDate = selectedDate,
                Price = priceValue,
                UtcTime = utcTime,
                MovieScheduleCode = schdetailId,
                ArticleCode = articleCode

            };
            var paramJson = JsonConvert.SerializeObject(param);
            HttpContext.Session.SetString("cinmaValues", paramJson);
            if (!identificationResult.isValid && !identificationResult.isLoggedIn)
            {
                string validation = "Cinema";
                var validationJson = JsonConvert.SerializeObject(validation);
                HttpContext.Session.SetString("SignInInformation", validationJson);
                TempData["ErrorMessage"] = "Please login to proceed further.";
                return RedirectToAction("Index", "SignIn");

            }

        }
        var seatValuesJson = HttpContext.Session.GetString("cinmaValues");
        //HttpContext.Session.Remove("cinmaValues");
        if (seatValuesJson != null )
        {
            var seatValues = JsonConvert.DeserializeObject<SeatLayout>(seatValuesJson);

            spacecode = seatValues?.SpaceCode;
            companyTinNumber = seatValues?.CompanyTinNumber;
            branchCode = seatValues?.BranchCode;
            companyName = seatValues?.CompanyName;
            movieName = seatValues?.MovieName;
            movieCode = seatValues?.MovieCode;
            hallName = seatValues?.HallName;
            utcTime = seatValues?.UtcTime;
            dimension = seatValues?.Dimension;
            schdetailId = seatValues?.MovieScheduleCode;
            spaceType = seatValues?.SpaceType;
            selectedDate = seatValues?.SelectedDate;
            priceValue = seatValues?.Price;
            companyCode = seatValues?.CompanyCode;
            id = seatValues?.Id;
            speceId = seatValues.SpaceId;
            articleCode = seatValues?.ArticleCode;
        }
        else
        {
            TempData["ErrorMessage"] = "Session Has Expired Please Restart the Booking Process";
            return RedirectToAction("Index", "Home");
        }

        string key = "cinema_" + companyTinNumber + "_" + branchCode + "_" + schdetailId + "_" + phoneNumber;
        var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
        var _seatCacheClient = _httpClientFactory.CreateClient("HulubejeCache");
        var seats = new SeatLayouts();

        _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        HttpResponseMessage seatresponse = await _v7Client.GetAsync($"cinema/getcinemaseatarrangement?code={companyCode}&spaceId={speceId}&id={id}");
        if (seatresponse.IsSuccessStatusCode)
        {
            string seatReasponseData = await seatresponse.Content.ReadAsStringAsync();
            seats = JsonConvert.DeserializeObject<SeatLayouts>(seatReasponseData);
        }
        HttpResponseMessage seatCacheresponse = await _seatCacheClient.GetAsync($"getEntriesContainsKey?key={key}");
        if (seatCacheresponse.IsSuccessStatusCode && seats!=null)
        {
            string cache = await seatCacheresponse.Content.ReadAsStringAsync();
            seats.SeatStatus = JsonConvert.DeserializeObject<List<SeatStatus>>(cache);
            System.Diagnostics.Debug.WriteLine(cache);
        }
        if (seats?.Data !=null)
        {
            seats.Data.CompanyTinNumber = companyTinNumber;
            seats.Data.CompanyName = companyName;
            seats.Data.BranchCode = branchCode;
            seats.Data.MovieScheduleCode = schdetailId;
            seats.Data.PhoneNumber = phoneNumber;
            seats.Data.CompanyCode = companyCode;
            seats.Data.CompanyName = companyName;
            seats.Data.Dimension = dimension;
            seats.Data.HallName = hallName;
            seats.Data.UtcTime = utcTime;
            seats.Data.MovieName = movieName;
            seats.Data.SelectedDate = selectedDate;
            seats.Data.Price = priceValue;
            seats.Data.ArticleCode = articleCode;
        }

        return seats != null ? View(seats) : View(null);

    }


    [HttpPost]
    public async Task<IActionResult> FetchSeatStatus([FromBody] string seatCacheKey)
    {
        var _seatCacheClient = _httpClientFactory.CreateClient("HulubejeCache");
        string cache;

        HttpResponseMessage seatCacheresponse = await _seatCacheClient.GetAsync($"getEntriesContainsKey?key={seatCacheKey}");

        if (seatCacheresponse.IsSuccessStatusCode)
        {
            cache = await seatCacheresponse.Content.ReadAsStringAsync();

            // Deserialize the JSON string to a list of Seat objects
            var seats = JsonConvert.DeserializeObject<List<SeatStatus>>(cache);

            // Convert seats to a format suitable for JSON serialization
            var serializedSeats = seats.Select(seat => new
            {
                value = seat.Value.ToString(),
                status = seat.Status
            }).ToList();

            return Json(serializedSeats);
        }
        else
        {
            // Handle error response
            return BadRequest("Error fetching seat status from cache.");
        }
    }
}
