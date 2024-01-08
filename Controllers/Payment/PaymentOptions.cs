using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HulubejeBooking.Models.PaymentModels;

namespace Payment.Controllers
{
    public class PaymentOptions : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PaymentOption()
        {
            var paymentOptionsJson = HttpContext.Session.GetString("PaymentOptions");
            if (!string.IsNullOrEmpty(paymentOptionsJson))
            {
                var viewPayments = JsonConvert.DeserializeObject<List<PaymentOptionModel>>(paymentOptionsJson);
                var wrapper = new Wrapper
                {
                    PaymentOptions = viewPayments,  
                    Boa = new BoAModel()  
                };
                return View(wrapper);
            }
            else
            {
                return View(new PaymentOptionModel());
            }
        }



    }
}
