using HulubejeBooking.Models.PaymentModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HulubejeBooking.Controllers.Payment
{
    public class BookingPost : Controller
    {
        public IActionResult Index()
        {
            var paymentDoneModelJson = HttpContext.Session.GetString("PaymentDoneModel");
            HttpContext.Session.Remove("PaymentDoneModel");
            var paymentDoneModel = JsonConvert.DeserializeObject<PaymentValidation>(paymentDoneModelJson);

            return View(paymentDoneModel);
        }
    }
}
