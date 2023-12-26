using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using HulubejeBooking.Models.BusModels;
namespace HulubejeBooking.Controllers.BusController
{
    public class BusHomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public BusHomeController()
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://192.168.1.25:8092/api/")
            };

            _httpClient.DefaultRequestHeaders.Add("x-api-key", "9BE090F9-7F52-4297-93A1-32D03D361DE9");
        }

        public async Task<IActionResult> Index()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("operators/getalloperators");
            var busModel = new BusModel();
            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var companyData = JsonConvert.DeserializeObject<List<CompanyModel>>(responseData);
                HttpResponseMessage routeResponse = await _httpClient.GetAsync("routes/getallroutes");
                if(routeResponse.IsSuccessStatusCode)
                {
                    string routeResponseData = await routeResponse.Content.ReadAsStringAsync();
                    var routeData = JsonConvert.DeserializeObject<RouteConfiguration>(routeResponseData);
                    busModel.RouteConfiguration = routeData;
                }
                busModel.Company = companyData;
                //busModel.C = RouteData.C;
                return View(busModel);
            }

            return View(null);

        }

    }
}
