using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.BusModels;
using HulubejeBooking.Models.CInemaModels;
using HulubejeBooking.Models.HotelModels;
using HulubejeBooking.Models.PaymentModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Common;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Transactions;

namespace HulubejeBooking.Controllers.Payment
{
    public class PaymentHomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;

        public PaymentHomeController(IHttpClientFactory httpClientFactory, IHttpContextAccessor? httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> IndexAsync([FromBody] PaymentModel model)
        {
            if (model != null) {
                var modelJson = JsonConvert.SerializeObject(model);
                HttpContext.Session.SetString("PaymentModels", modelJson);
            }
            var userDataCookie = _httpContextAccessor?.HttpContext?.Request.Cookies[CNET_WebConstants.IdentificationCookie];
            string? code;
            if (!string.IsNullOrEmpty(userDataCookie))
            {
                var user = JsonConvert.DeserializeObject<UserInformation>(userDataCookie);
                code = user?.phoneNumber;
                }
            else
                {
                return BadRequest();
                    }
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            if (HttpContext.Session.TryGetValue("loginToken", out var loginToken))
                    {
                string token = Encoding.UTF8.GetString(loginToken);
                _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage transactionIdResponse = await _v7Client.GetAsync($"payment/generatetransactionid?code={code}");
                if (transactionIdResponse.IsSuccessStatusCode)
                        {
                    string transactionIdData = await transactionIdResponse.Content.ReadAsStringAsync();
                    var transactionId = JsonConvert.DeserializeObject<TransactionId>(transactionIdData);
                    if (transactionId != null)
                        {
                        HttpContext.Session.SetString("VoucherCode", transactionId.Data);
                        }
                    HttpResponseMessage paymentOptionsResponse = await _v7Client.GetAsync($"payment/getuserpaymnetoption?code={code}&companyCode={model?.CompanyCode}&branchCode={model?.BranchCode}");
                    if (paymentOptionsResponse.IsSuccessStatusCode)
                        {
                        string paymentoptionData = await paymentOptionsResponse.Content.ReadAsStringAsync();
                        HttpContext.Session.SetString("paymentOptrions", paymentoptionData);
                    }

                    return Ok();
                        }
                    else
                    {
                    return BadRequest();
                    }

                }
                else
                {
                TempData["ErrorMessage"] = "Session Has Expired Please Restart the Booking Process";
                return RedirectToAction("Index", "Home");
                }
            }

            {
                return Error();
            }
        }
        [HttpPost]
        public async Task<IActionResult> SelectedOptionAsync([FromBody] RequestWrapper data)
        {
            try
            {
                var operationMode = data.FlagData.operationMode;
                var value = OperationModeController.FlagChecker(operationMode);
                var synch = value.Synchronous;
                var asynch = value.Asynchronous;
                var Currency = "etb";

                var vCode = HttpContext.Session.GetString("VoucherCode");
                var TransactionId = !string.IsNullOrWhiteSpace(vCode)
                    ? JsonConvert.DeserializeObject<string>(vCode)
                    : null;

                var parameters = HttpContext.Session.GetString("paymentInfo");
                var newParam = !string.IsNullOrWhiteSpace(parameters)
                    ? JsonConvert.DeserializeObject<AuthorizePayment>(parameters)
                    : null;



                if (!HttpContext.Session.TryGetValue("AccessToken", out var accessTokenBytes))
                {
                    return Error();
                }

                string accessToken = Encoding.UTF8.GetString(accessTokenBytes);

                using var _client = _httpClientFactory.CreateClient("Payment");
                if (synch)
                {
                    var param = new
                    {
                        newParam?.UserMobileNumber,
                        newParam?.SupplierTin,
                        newParam?.SupplierOUD,
                        TransactionId,
                        data.AuthorizePaymentData.PaymentProviderOUD,
                        newParam?.Amount,
                        AdditionalParameters = new
                        {
                            data.AuthorizePaymentData.AdditionalParameters?.ReferenceNumber
                        }
                    };


                    var jsonRequest = JsonConvert.SerializeObject(param);

                    var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "payment/authorization", content);

                    if (response.IsSuccessStatusCode)
                    {



                        var transactionModel = new
                        {
                            newParam?.UserMobileNumber,
                            newParam?.SupplierTin,
                            newParam?.SupplierOUD,
                            TransactionId,
                            data.AuthorizePaymentData.PaymentProviderOUD,
                            newParam?.Amount,
                            AdditionalParameters = new
                            {
                                data.AuthorizePaymentData.AdditionalParameters?.ReferenceNumber
                            }
                        };
                        var values = JsonConvert.SerializeObject(transactionModel);

                        HttpContext.Session.SetString("transactionDatas", values);

                        return Json(new { cardText = "telebirr SMS OTP" });
                    }
                    else
                    {
                        return Json(new { cardText = "telebirr SMS OTP" });
                    }
                }
                if (asynch)
                {
                    var param = new
                    {
                        newParam?.UserMobileNumber,
                        newParam?.SupplierTin,
                        newParam?.SupplierOUD,
                        TransactionId,
                        data.TransactionData.PaymentProviderOUD,
                        newParam?.Amount,
                        data.TransactionData.Pin,
                        AdditionalParameters = new
                        {
                            data.TransactionData.AdditionalParameters?.ReferenceNumber
                        }
                    };

                    var jsonRequest = JsonConvert.SerializeObject(param);
                    var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                    var transactionVerification = new
                    {
                        newParam?.SupplierOUD,
                        TransactionId,
                        newParam?.Amount
                    };
                    var values = JsonConvert.SerializeObject(param);

                    HttpContext.Session.SetString("paymentValidation", values);

                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "payment/transaction", content);
                    string responseData = await response.Content.ReadAsStringAsync();

                    //var paymentValidation = JsonConvert.DeserializeObject<PaymentValidation>(responseData);

                    HttpContext.Session.SetString("ValidationInfo", responseData);

                    if (response.IsSuccessStatusCode)
                    {


                        return Json(new { cardText = "telebirr USSD Push" });

                    }
                }
                if (!synch && !asynch)
                {
                    var param = new
                    {
                        newParam?.UserMobileNumber,
                        newParam?.SupplierTin,
                        newParam?.SupplierOUD,
                        TransactionId,
                        data.TransactionData.PaymentProviderOUD,
                        newParam?.Amount,
                        data.TransactionData.Pin,
                        Currency

                    };
                    var jsonRequest = JsonConvert.SerializeObject(param);
                    var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "payment/transaction", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonConvert.DeserializeObject<BoAResponse>(responseData);

                        var cardText = "BOA Card Payment";
                        var redirectUrl = apiResponse?.additionalParameters?.RedirectUrl;


                        var paymentResponse = new PaymentResponse
                        {
                            cardText = cardText,
                            RedirectUrl = redirectUrl
                        };

                        return Json(paymentResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                
            }

            return Error();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}
