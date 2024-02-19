using HulubejeBooking.Models;
using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.BusModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace HulubejeBooking.Controllers
{
    public class HistoryController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;

        public HistoryController(IHttpClientFactory httpClientFactory, IHttpContextAccessor? httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> IndexAsync(string phoneNumber)
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
