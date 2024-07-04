using HulubejeBooking.Models;
using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.BusModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using HulubejeBooking.Controllers.Authentication;
using Tweetinvi.Core.Models;
using NuGet.Common;
using System.Net.Http.Headers;
using System.Text;
namespace HulubejeBooking.Controllers
{
    public class HistoryController : Controller
    {
        private readonly AuthenticationManager _authenticationManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;

        public HistoryController(IHttpClientFactory httpClientFactory, IHttpContextAccessor? httpContextAccessor, AuthenticationManager authenticationManager)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
        }

        public async Task<IActionResult> IndexAsync(string? phoneNumber)
        {
            string? token = "";
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
                phoneNumber = identificationResult?.UserData?.Code;
                token = identificationResult?.UserData?.Token;
            }
            if (identificationResult != null && !(identificationResult.isLoggedIn || identificationResult.isValid))
            {
                return RedirectToAction("Index", "home");
            }
            var historyWrapper = new HistoryWrapper();
            var busClient = _httpClientFactory.CreateClient("BusBooking");
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            HttpResponseMessage response = await busClient.GetAsync($"history/gethistorybyphoneNumber?PhoneNumber={phoneNumber}");
            if (response.IsSuccessStatusCode)
            {
                string busresponseData = await response.Content.ReadAsStringAsync();
                var busHistory = JsonConvert.DeserializeObject<List<HistoryModel>>(busresponseData);
                historyWrapper.HistoryModel = busHistory;
            }
            _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage historyResponse = await _v7Client.GetAsync($"voucher/gethistory?code={phoneNumber}");
            if (historyResponse.IsSuccessStatusCode)
            {
                string responseData = await historyResponse.Content.ReadAsStringAsync();
                var history = JsonConvert.DeserializeObject<OrdersModel>(responseData);
                historyWrapper.OrdersModel = history;
            }
            return View(historyWrapper);

        }
        public async Task<IActionResult> SubmitRating([FromBody] Ratings? rating)
        {
            string? token = "";
            string? phoneNumber = "";

            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            var identificationResult = await _authenticationManager.identificationValid();
            if (identificationResult != null)
            {
                phoneNumber = identificationResult?.UserData?.Code;
                token = identificationResult?.UserData?.Token;
            }
            if (rating != null)
            {
                rating.Code = phoneNumber;
                rating.BranchCode = 45;
            }
            var jsonBody = JsonConvert.SerializeObject(rating);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            if (rating == null) { return BadRequest("Error"); }
            else
            {
                _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage ratingResponse = await _v7Client.PostAsync($"review/save", content);
                if (ratingResponse.IsSuccessStatusCode)
                {
                    string responseData = await ratingResponse.Content.ReadAsStringAsync();
                    var review = JsonConvert.DeserializeObject<RatingResponse>(responseData);
                    return Json(review);
                }
                return BadRequest("Error");
            }

        }    
    }
}
