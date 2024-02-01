using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using HulubejeBooking.Models.BusModels;
namespace HulubejeBooking.Controllers.BusController
{
    public class BusHomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BusHomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var busSeatLayoutClient = _httpClientFactory.CreateClient("BusBooking");
            HttpResponseMessage response = await busSeatLayoutClient.GetAsync("operators/getalloperators");
            var busModel = new BusModel();
            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var companyData = JsonConvert.DeserializeObject<List<CompanyModel>>(responseData);
                busModel.Company = companyData;
            }
            HttpResponseMessage routeResponse = await busSeatLayoutClient.GetAsync("routes/getallroutes");
            if(routeResponse.IsSuccessStatusCode)
            {
                string routeResponseData = await routeResponse.Content.ReadAsStringAsync();
                var routeData = JsonConvert.DeserializeObject<List<RouteModel>>(routeResponseData);
                busModel.Routes = routeData;
            }
            return View(busModel);
        }

    }
}
