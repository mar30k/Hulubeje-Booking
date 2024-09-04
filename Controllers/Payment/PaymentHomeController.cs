using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.BusModels;
using HulubejeBooking.Models.CInemaModels;
using HulubejeBooking.Models.HotelModels;
using HulubejeBooking.Models.PaymentModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Common;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Transactions;

namespace HulubejeBooking.Controllers.Payment
{
    public class PaymentHomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor? _httpContextAccessor;
        private readonly AuthenticationManager _authenticationManager;

        public PaymentHomeController(IHttpClientFactory httpClientFactory, IHttpContextAccessor? httpContextAccessor, AuthenticationManager authenticationManager)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
        }
        public async Task<IActionResult> IndexAsync([FromBody] PaymentModel model)
        {
            var b = await _authenticationManager.identificationValid();

            if (model != null) {
                var modelJson = JsonConvert.SerializeObject(model);
                HttpContext.Session.SetString("PaymentModels", modelJson);
            }
            string? code = b.UserData.Code;

            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            string? token = b?.UserData?.Token;
            _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage transactionIdResponse = await _v7Client.GetAsync($"payment/generatetransactionid?code={code}");
            if (transactionIdResponse.IsSuccessStatusCode)
            {   
                string transactionIdData = await transactionIdResponse.Content.ReadAsStringAsync();
                var transactionId = JsonConvert.DeserializeObject<TransactionId>(transactionIdData);
                if (transactionId != null && transactionId.Data!=null)
                {
                    HttpContext.Session.SetString("VoucherCode", transactionId.Data);
                }
                HttpResponseMessage paymentOptionsResponse = await _v7Client.GetAsync($"payment/getuserpaymnetoption?code={code}&companyCode={model?.CompanyCode}&branchCode={model?.BranchCode}");
                if (paymentOptionsResponse.IsSuccessStatusCode)
                {
                    string paymentoptionData = await paymentOptionsResponse.Content.ReadAsStringAsync();
                    HttpContext.Session.SetString("paymentOptions", paymentoptionData);
                }

                return Ok();
            }
            else 
            { 
                return BadRequest();
            }
        }

        public async Task<IActionResult> SelectedOption([FromBody] PaymentProcessorData data)
        {
            if (data != null)
            {
                var paymentProcessorJson = JsonConvert.SerializeObject(data);
                HttpContext.Session.SetString("PaymentProcessorData", paymentProcessorJson);
            }
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            var b = await _authenticationManager.identificationValid();
            string? code = b.UserData.Code;
            string? token = b?.UserData?.Token;
            var vCode = HttpContext.Session.GetString("VoucherCode");

            if (HttpContext.Session.TryGetValue("PaymentModels", out var model) && !string.IsNullOrWhiteSpace(vCode))
            {
                string paymentModel = Encoding.UTF8.GetString(model);
                var paymentModelData = JsonConvert.DeserializeObject<PaymentModel>(paymentModel);
                _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var additionalParameters = new
                {
                    referenceNumber = "optional"
                };
                var ActivityLog = new ActivityLog
                {
                    Code = code,
                };
                var param = new
                {
                    ActivityLog,
                    additionalParameters,
                    userMobileNumber = code,
                    transactionId = vCode,
                    supplierConsigneeUnit = paymentModelData?.BranchCode,
                    supplierConsigneeId = paymentModelData?.CompanyCode,
                    amount = paymentModelData?.Amount,
                    paymentProcessorConsigneeUnit = data?.PaymentProcessorConsigneeUnit,
                    paymentProcessorConsigneeId = data?.PaymentProcessorConsigneeId,
                    paymentProcessorUnitName = data?.PaymentProcessorName,
                    //amount = 0.1,
                    operationmode = data?.OperationMode,
                    pin = "",
                };
                HttpContext.Session.SetString("PaymentTransactionRequest", JsonConvert.SerializeObject(param));
                HttpContext.Session.SetString("ActivityLog", JsonConvert.SerializeObject(ActivityLog));
                var content = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                HttpResponseMessage authorizationResponse = await _v7Client.PostAsync($"payment/authorization", content);
                if (authorizationResponse.IsSuccessStatusCode)
                {
                    string authorizationData = await authorizationResponse.Content.ReadAsStringAsync();
                    var authorizarion = JsonConvert.DeserializeObject<PaymentAuthorizationResponse>(authorizationData);
                    HttpContext.Session.SetString("PaymentAuthorizationResponse", authorizationData);
                    return Json(authorizarion);
                }
                else
                {
                    return BadRequest(); 
                }


            }
            else
            {
                return BadRequest();
            }
        }
    }
}
