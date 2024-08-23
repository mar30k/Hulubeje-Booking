using CNET_ERP_V7_VoucherPrintDialogue.Models;
using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models;
using HulubejeBooking.Models.Temp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace HulubejeBooking.Controllers
{
    public class ReserveTableController : Controller
    {
        private readonly AuthenticationManager _authenticationManager;
        private readonly IHttpClientFactory _httpClientFactory;
        public ReserveTableController(AuthenticationManager authenticationManager, IHttpClientFactory httpClientFactory)
        {
            _authenticationManager = authenticationManager;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Index()
        {
            var identificationResult = await _authenticationManager.identificationValid();

            string? token = identificationResult?.UserData?.Token;
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var getCompany = new HulubejeResponse<GetCompany>();
            HttpResponseMessage getCompanyResponse = await _v7Client.GetAsync($"tablereservation/get?company={23}&branch={27}");
            if (getCompanyResponse.IsSuccessStatusCode)
            {
                string responseData = await getCompanyResponse.Content.ReadAsStringAsync();
                getCompany = responseData != null ? JsonConvert.DeserializeObject<HulubejeResponse<GetCompany>>(responseData) : new HulubejeResponse<GetCompany>();
            }
            var companysetting = new HulubejeResponse<GetCompanySetting>();
            HttpResponseMessage companysettingResponse = await _v7Client.GetAsync($"routing/getcompanysetting?companyCode={23}&industryType=-10");
            if (companysettingResponse.IsSuccessStatusCode)
            {
                string responseData = await companysettingResponse.Content.ReadAsStringAsync();
                companysetting = responseData != null ? JsonConvert.DeserializeObject<HulubejeResponse<GetCompanySetting>>(responseData) : new HulubejeResponse<GetCompanySetting>();
            }
            var reserveTable = new ReserveTable
            {
                GetCompany = getCompany,
                GetCompanySetting = companysetting
            };
            return View(reserveTable);
        }
        [HttpPost]
        public IActionResult Index(ReservationModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("ReservationConfirmation");
            }

            // Inspecting the validation errors
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"Key: {state.Key}, Error: {error.ErrorMessage}");
                }
            }

            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

            return View();
        }
    }
}
