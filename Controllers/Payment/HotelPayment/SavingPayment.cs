using Microsoft.AspNetCore.Mvc;
using HulubejeBooking.Models.PaymentModels;

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
            HttpContext.Session.SetString("Data", validationJson);

            return Json(new RequestWrapper());
        }

        public async Task<IActionResult> PaymentCommonAsync()
        {
            var _Hotel = _httpClientFactory.CreateClient("CnetHulubeje");

            var data = HttpContext.Session.GetString("Data");
            var paymentInfo = HttpContext.Session.GetString("PaymentInfo");
            var validationInfo = HttpContext.Session.GetString("ValidationInfo");

            HttpContext.Session.Remove("Data");
            HttpContext.Session.Remove("PaymentInfo");
            HttpContext.Session.Remove("ValidationInfo");


            var newData = JsonConvert.DeserializeObject<RequestWrapper>(data);
            var PaymentInfo = JsonConvert.DeserializeObject<PaymentInfoModel>(paymentInfo);
            var newValidationInfo = JsonConvert.DeserializeObject<PaymentValidation>(validationInfo);
            var PaymentMethod = "Telebirr OTP";

            if (newData?.CinemaDetailsData != null)
            {
                var b = newData.CinemaDetailsData;
                var param = new
                {
                    b.Consignee,
                    b.GrandTotal,
                    b.MovieSchedule,
                    b.SeatsToBook,
                    b.OrgUnitDef,
                    b.OrgTin,
                    newValidationInfo?.transactionReference,
                    b.CinemaArticles,
                    b.OnBookSuccess,
                    b.Latitude,
                    b.Longitude,
                    b.Platform,
                    PaymentInfo,
                    PaymentMethod
                };
                var paramBody = JsonConvert.SerializeObject(param);
                var roomContent = new StringContent(paramBody, Encoding.UTF8, "application/json");

                var response = await _Hotel.PostAsync(_Hotel.BaseAddress + "/CinemaBook/BookSeat", roomContent);

                var responseData = await response.Content.ReadAsStringAsync();

                var PaymentDone = JsonConvert.DeserializeObject<PaymentValidation>(responseData);

                HttpContext.Session.SetString("PaymentDoneModel", JsonConvert.SerializeObject(PaymentDone));

            }

            else if (newData?.GuestInfoData != null)
            {
                var b = newData.GuestInfoData;
                var dt = DateRangeParser.parseDateRange(b.date);
                var arrivalDate = dt.startDateString;
                var departureDate = dt.endDateString;
                var cashRecieptVoucher = PaymentInfo?.PaymentTransactionRequest.TransactionId;
               var  param = new
                {
                    b.orgTin,
                    voucherCode = PaymentInfo?.PaymentTransactionRequest.TransactionId,
                    arrivalDate,
                    departureDate,
                    b.adult,
                    b.child,
                    b.roomTypeCode,
                    b.rateCode,
                    b.rateCodeDetail,
                    averageAmount = "0.1",
                    totalAmount = "0.1",
                    paymentMethod = PaymentInfo?.PaymentTransactionRequest.PaymentProviderOUD,
                    b.roomCount,
                    b.guests,
                    b.oud,
                    b.specialRequirement,
                    cashRecieptVoucher,
                    newValidationInfo?.transactionReference,
                    b.onHotelBookSuccess,
                    PaymentInfo

                };
                var paramBody = JsonConvert.SerializeObject(param);
                var roomContent = new StringContent(paramBody, Encoding.UTF8, "application/json");

                var response = await _Hotel.PostAsync(_Hotel.BaseAddress + "/HotelBook/BookHotelRoom", roomContent);

                var responseData = await response.Content.ReadAsStringAsync();

                var PaymentDone = JsonConvert.DeserializeObject<PaymentValidation>(responseData);


                HttpContext.Session.SetString("PaymentDoneModel", JsonConvert.SerializeObject(PaymentDone));

                


            }

            return RedirectToAction("Index", "BookingPost");
        }



  
    }
}
