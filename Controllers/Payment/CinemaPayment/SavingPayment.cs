using Microsoft.AspNetCore.Mvc;
using HulubejeBooking.Models.PaymentModels;
using HulubejeBooking.Models.PaymentModels.HotlePaymentModels;
using HulubejeBooking.Models;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Net.Http.Headers;
using Payment.Controllers;

namespace HulubejeBooking.Controllers.Payment.HotlePayment
{
    public class SavingPayment : Controller
    {
        private readonly ILogger<SavingPayment> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public SavingPayment(ILogger<SavingPayment> logger, IHttpClientFactory httpClientFactory)
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
            var validation = data.CinemaDetailsData;
            var validationJson = JsonConvert.SerializeObject(validation);
            HttpContext.Session.SetString("ValidationData", validationJson);
            
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
