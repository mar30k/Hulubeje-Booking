using Microsoft.AspNetCore.Mvc;
using HulubejeBooking.Models.PaymentModels;
using HulubejeBooking.Models;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Net.Http.Headers;
using Payment.Controllers;
using HulubejeBooking.Models.HotelModels;
using HulubejeBooking.Controllers.HotelController;

namespace HulubejeBooking.Controllers.Payment.HotlePayment
{
    public class SHotelpayment : Controller
    {
        private readonly ILogger<SHotelpayment> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public SHotelpayment(ILogger<SHotelpayment> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {

            return View();
        }
        public async Task<IActionResult> PaymentAsync([FromBody] RequestWrapper data)
        {
            var validation = new RequestWrapper 
            {
                GuestInfoData = data.GuestInfoData,
                CinemaDetailsData = data.CinemaDetailsData
            };
            var validationJson = JsonConvert.SerializeObject(validation);
            HttpContext.Session.SetString("ValidationData", validationJson);

            return Json(new RequestWrapper());
        }

        public async Task<IActionResult> PaymentCommonAsync()
        {
            var _Hotel = _httpClientFactory.CreateClient("HotelBooking");

            var data = HttpContext.Session.GetString("ValidationData");
            var paymentInfo = HttpContext.Session.GetString("PaymentInfo");
            var validationInfo = HttpContext.Session.GetString("ValidationInfo");
            HttpContext.Session.Remove("ValidationData");
            HttpContext.Session.Remove("PaymentInfo");
            HttpContext.Session.Remove("ValidationInfo");


            var newData = JsonConvert.DeserializeObject<RequestWrapper>(data);
            var newPaymentInfo = JsonConvert.DeserializeObject<PaymentInfoModel>(paymentInfo);
            var newValidationInfo = JsonConvert.DeserializeObject<PaymentValidation>(validationInfo);

            object param = null; 

            if (newData.CinemaDetailsData != null)
            {
                var b = newData.CinemaDetailsData;
                param = new
                {
                    b.Consignee,
                    b.GrandTotal,
                    b.MovieSchedule,
                    b.SeatsToBook,
                    b.OrgUnitDef,
                    b.OrgTin,
                    newValidationInfo.transactionReference,
                    b.CinemaArticles,
                    b.OnBookSuccess,
                    b.Latitude,
                    b.Longitude,
                    b.Platform,
                    newPaymentInfo 
                };

            }

            else if (newData.GuestInfoData != null)
            {
                var b = newData.GuestInfoData;
                var dt = DateRangeParser.parseDateRange(b.date);
                var arrivalDate = dt.startDateString;
                var departureDate = dt.endDateString;
                param = new
                {
                    b.orgTin,
                    newPaymentInfo.PaymentTransactionRequest.TransactionId,
                    arrivalDate,
                    departureDate,
                    b.adult,
                    b.child,
                    b.roomTypeCode,
                    b.rateCode,
                    b.rateCodeDetail,
                    averageAmount = "0.1",
                    totalAmount = "0.1",
                    newPaymentInfo.PaymentTransactionRequest.PaymentProviderOUD,
                    b.roomCount,
                    b.guests,
                    b.oud,
                    b.specialRequirement,
                    newValidationInfo.transactionReference,
                    b.onHotelBookSuccess,
                    newPaymentInfo

                };
                var paramBody = JsonConvert.SerializeObject(param);
                var roomContent = new StringContent(paramBody, Encoding.UTF8, "application/json");

                var response = await _Hotel.PostAsync(_Hotel.BaseAddress + "/HotelBook/BookHotelRoom", roomContent);

                var responseData = await response.Content.ReadAsStringAsync();

                PaymentValidation PaymentDone = JsonConvert.DeserializeObject<PaymentValidation>(responseData);



            }
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
