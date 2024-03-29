using HulubejeBooking.Models;
using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.BusModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using HulubejeBooking.Controllers.Authentication;
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
            if (!(identificationResult.isLoggedIn || identificationResult.isValid))
            {
                return RedirectToAction("Index", "home");
            }

            var historyWrapper = new HistoryWrapper();
            var busClient = _httpClientFactory.CreateClient("BusBooking");
            var hulubejeClient = _httpClientFactory.CreateClient("CnetHulubeje");
            HttpResponseMessage response = await busClient.GetAsync($"history/gethistorybyphoneNumber?PhoneNumber={phoneNumber}");
            if (response.IsSuccessStatusCode)
            {
                string busresponseData = await response.Content.ReadAsStringAsync();
                var busHistory = JsonConvert.DeserializeObject<List<HistoryModel>>(busresponseData);
                historyWrapper.HistoryModel = busHistory;
            }
            HttpResponseMessage historyResponse = await hulubejeClient.GetAsync(hulubejeClient.BaseAddress + $"/order/GetOrdersByConsigneeCode?consigneeCode={phoneNumber}&page=1");
            if (historyResponse.IsSuccessStatusCode)
            {
                string responseData = await historyResponse.Content.ReadAsStringAsync();
                var history = JsonConvert.DeserializeObject<OrdersModel>(responseData);
                historyWrapper.OrdersModel = history;

                while (history?.NextPage != null)
                {
                    historyResponse = await hulubejeClient.GetAsync(hulubejeClient.BaseAddress + $"/order/GetOrdersByConsigneeCode?consigneeCode={phoneNumber}&page={history?.NextPage}");
                    if (historyResponse.IsSuccessStatusCode)
                    {
                        responseData = await historyResponse.Content.ReadAsStringAsync();
                        history = JsonConvert.DeserializeObject<OrdersModel>(responseData);
                        historyWrapper?.OrdersModel?.Orders?.AddRange(history?.Orders ?? Enumerable.Empty<Orders>());
                        if (historyWrapper != null && historyWrapper.OrdersModel != null && history != null)
                        {
                            historyWrapper.OrdersModel.NextPage = history.NextPage;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return View(historyWrapper);

        }
    }
}
