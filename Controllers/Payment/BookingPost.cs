using HulubejeBooking.Models.PaymentModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HulubejeBooking.Controllers.Payment
{
    public class BookingPost : Controller
    {
        public IActionResult Index()
        {
            // Retrieving object from session
            var paymentDoneModelJson = HttpContext.Session.GetString("PaymentDoneModel");
            var paymentDoneModel = JsonConvert.DeserializeObject<PaymentValidation>(paymentDoneModelJson);
            // Use paymentDoneModel as needed

            return View(paymentDoneModel);
        }
    }
}
