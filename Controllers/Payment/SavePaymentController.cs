using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.PaymentModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using System.Net.Http.Headers;
using System.Text;

namespace HulubejeBooking.Controllers.Payment
{
    public class SavePaymentController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;
        private readonly AuthenticationManager _authenticationManager;
        public SavePaymentController(IHttpClientFactory httpClientFactory, IHttpContextAccessor? httpContextAccessor, AuthenticationManager authenticationManager)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
        }
        public class JsonDeserializationHelper
        {
            public static T Deserialize<T>(string json) where T : new()
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(json) ?? new T();
                }
                catch (JsonException)
                {
                    return new T();
                }
            }
        }
        public async Task<IActionResult> IndexAsync([FromBody] string pin)
        {
            var b = await _authenticationManager.identificationValid();
            string? token = b?.UserData?.Token;

            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            var saveResponse = new SaveResponse
            {
                ErrorMessages = new List<string>{"Unknown Error Occured. Please try again Later!"}
            };
            if (
                HttpContext.Session.TryGetValue("PaymentModels", out var paymentModelBytes) &&
                HttpContext.Session.TryGetValue("ActivityLog", out var activityLogBytes) &&
                HttpContext.Session.TryGetValue("PaymentTransactionRequest", out var paymentTransactionRequestBytes)&&
                HttpContext.Session.TryGetValue("PaymentProcessorData", out var paymentProcessorBytes) &&
                HttpContext.Session.TryGetValue("PaymentAuthorizationResponse", out var authorizatioBytes) 
                )
            {
                try
                {
                    var paymentModelJson = Encoding.UTF8.GetString(paymentModelBytes);
                    var authorizatioJson = Encoding.UTF8.GetString(authorizatioBytes);
                    var activityLogJson = Encoding.UTF8.GetString(activityLogBytes);
                    var paymentTransactionRequestJson = Encoding.UTF8.GetString(paymentTransactionRequestBytes);
                    var paymentProcessorJson = Encoding.UTF8.GetString(paymentProcessorBytes);

                    var paymentModelData = paymentModelJson != null?JsonDeserializationHelper.Deserialize<PaymentModel>(paymentModelJson):new PaymentModel();
                    var authorizatioData = authorizatioJson != null ? JsonDeserializationHelper.Deserialize<PaymentAuthorizationResponse>(authorizatioJson) : new PaymentAuthorizationResponse();
                    var activityLog = activityLogJson != null ? JsonDeserializationHelper.Deserialize<ActivityLog>(activityLogJson) : new ActivityLog();
                    var paymentTransactionRequest = paymentTransactionRequestJson != null ? JsonDeserializationHelper.Deserialize<PaymentTransactionRequest>(paymentTransactionRequestJson) : new PaymentTransactionRequest();
                    PaymentProcessorData paymentProcessorData = paymentProcessorJson != null ? JsonDeserializationHelper.Deserialize<PaymentProcessorData>(paymentProcessorJson) : new PaymentProcessorData();


                    _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    if (paymentModelData != null)
                    {
                        paymentModelData.Code = activityLog?.Code;
                        paymentModelData.PaymentInfo = new PaymentInfo
                        {
                            Type = authorizatioData?.AdditionalParameters?.Type,
                            IsAsyncMode = authorizatioData?.AdditionalParameters?.IsAsyncMode?.ToString(),
                            PaymentTransactionRequest = paymentTransactionRequest ?? new PaymentTransactionRequest()
                        };

                        if (paymentModelData.PaymentInfo.PaymentTransactionRequest != null)
                        {
                            paymentModelData.PaymentInfo.PaymentTransactionRequest.Pin = pin;
                        }
                        paymentModelData.ActivityLog = activityLog;
                        paymentModelData.PaymentMethod = paymentProcessorData?.PaymentProcessorName;
                        var content = JsonConvert.SerializeObject(paymentModelData);
                        var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
                        HttpResponseMessage saveResponseRequest = await _v7Client.PostAsync("voucher/save", httpContent);
                        if (saveResponseRequest.IsSuccessStatusCode)
                        {
                            var savaResponseData = await saveResponseRequest.Content.ReadAsStringAsync();
                            saveResponse = savaResponseData != null ? JsonDeserializationHelper.Deserialize<SaveResponse>(savaResponseData) : new SaveResponse();
                        }
                        return Json(saveResponse);
                    }
                    else
                    {
                        return BadRequest();
                    }

                }
                catch (Exception ex)    
                {
                    throw new Exception("An error occurred while processing session data.", ex);
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Session Has Expired Please Restart the Booking Process";
                return RedirectToAction("Index", "Home");
            }

        }
    }
}
