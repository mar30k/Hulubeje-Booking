using CNET_ERP_V7_VoucherPrintDialogue.Models;
using CNET_V7_Domain.Domain.CommonSchema;
using CNET_V7_Domain.Domain.SettingSchema;
using CNET_V7_Domain.Misc;
using CNET_V7_Domain.Misc.CommonTypes;
using CNET_V7_Domain.Misc.ThirdParty;
using Cnetv7BufferHolder;
using DevExpress.Pdf.Native.BouncyCastle.Asn1.Ocsp;
using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Helpers;
using HulubejeBooking.Models;
using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.BusModels;
using HulubejeBooking.Models.EventModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NuGet.Common;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using Tweetinvi.Core.Models;
using static QRCoder.PayloadGenerator;


namespace HulubejeBooking.Controllers
{
    public class HistoryController : Controller
    {
        private readonly IQRCodeGeneratorService _qrCodeGeneratorService;
        private readonly AuthenticationManager _authenticationManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;
        private HulubejeBooking.Helpers.SharedHelpers _sharedHelpers;
        private readonly InitialBufferPopulator _initialBufferPopulator;
        private static IOptions<OtherSettings> _ftpSettings;
        public HistoryController(IHttpClientFactory httpClientFactory, IHttpContextAccessor? httpContextAccessor, AuthenticationManager authenticationManager, IQRCodeGeneratorService qRCodeGeneratorService, HulubejeBooking.Helpers.SharedHelpers sharedHelpers, IOptions<OtherSettings> otherSettings, InitialBufferPopulator initialBufferPopulator)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
            _qrCodeGeneratorService = qRCodeGeneratorService;
            _sharedHelpers = sharedHelpers;
            _ftpSettings = otherSettings;
            _initialBufferPopulator = initialBufferPopulator;
        }

        public async Task<IActionResult> Index(string? phoneNumber)
        {
            string? token = "";
            var identificationResult = await _authenticationManager.identificationValid();
            if (identificationResult != null)
            {
                ViewBag.isVaild = identificationResult.isValid;
                ViewBag.isLoggedIn = identificationResult.isLoggedIn;
                ViewBag.FirstName = identificationResult?.UserData.FirstName;
                ViewBag.LastName = identificationResult?.UserData.LastName;
                ViewBag.MiddleName = identificationResult?.UserData.MiddleName;
                ViewBag.Personalattachment = identificationResult?.UserData.PersonalAttachment;
                ViewBag.Idnumber = identificationResult?.UserData.IdNumber;
                ViewBag.Idtype = identificationResult?.UserData.IdType;
                ViewBag.Dob = identificationResult?.UserData.Dob;
                ViewBag.Idattachment = identificationResult?.UserData.IdAttachment;
                ViewBag.PhoneNumber = identificationResult?.UserData.Code;
                ViewBag.EmailAddress = identificationResult?.UserData.Email;
                phoneNumber = identificationResult?.UserData?.Code;
                token = identificationResult?.UserData?.Token;
            }
            if (identificationResult != null && !(identificationResult.isLoggedIn || identificationResult.isValid))
            {
                return RedirectToAction("Index", "home");
            }
            var historyWrapper = new HistoryWrapper();
            var busClient = _httpClientFactory.CreateClient("BusBooking");
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            HttpResponseMessage response = await busClient.GetAsync($"history/gethistorybyphoneNumber?PhoneNumber={phoneNumber}");
            if (response.IsSuccessStatusCode)
            {
                string busresponseData = await response.Content.ReadAsStringAsync();
                var busHistory = JsonConvert.DeserializeObject<List<HistoryModel>>(busresponseData);
                historyWrapper.HistoryModel = busHistory;
            }
            _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string phoneParam = $"?code={phoneNumber}";
            historyWrapper.OrdersModel = await GetHistoryAsync($"voucher/gethistory{phoneParam}", 1);
            historyWrapper.PayementsHistory = await GetHistoryAsync($"voucher/getpaymenthistory{phoneParam}", 1);
            return View(historyWrapper);

        }


        public async Task<HulubejeResponse<List<VoucherData>>?> GetHistoryAsync(string endpoint, int page)
        {
            var identificationResult = await _authenticationManager.identificationValid();
            string? token = identificationResult.UserData?.Token;
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _v7Client.GetAsync($"{endpoint}&page={page}");
            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<HulubejeResponse<List<VoucherData>>>(responseData);
            }
            return null;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaymentHistoryPartial(string endPoint, string phone, int page)
        {
            if(endPoint.ToLower() == "gethistory") { ViewData["IsOrderHistory"] = true; }
            var result = await GetHistoryAsync($"voucher/{endPoint}?code={phone}", page);

            if (result?.Data == null || !result.Data.Any())
            {
                return Content("");
            }

            return PartialView("_VoucherHistoryPartial", result.Data);
        }

        public async Task<IActionResult> GetReviews([FromBody] GetReviewsRequest request)
        {
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            string? token = "";
            var review = new HulubejeResponse<ReviewResponse>();

            //request.BranchCode = 55915;
            var identificationResult = await _authenticationManager.identificationValid();
            if (identificationResult != null)
            {
                token = identificationResult?.UserData?.Token;
            }

            var jsonBody = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            if (request == null)
            {
                return Json(new { success = false, message = "Invalid request body." });
            }

            _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage ratingResponse = await _v7Client.PostAsync($"review/get", content);

            if (ratingResponse.IsSuccessStatusCode)
            {
                string responseData = await ratingResponse.Content.ReadAsStringAsync();
                review = JsonConvert.DeserializeObject<HulubejeResponse<ReviewResponse>>(responseData);

                if(review!=null && review.Data!=null)
                {
                    review.Data.BranchCode = request.BranchCode;
                }
                // Render the partial view into a string
                var html = await this.RenderViewAsync("_CompanyReviews", review, true);
                return Json(new { success = true, html });
            }

            return Json(new { success = false, message = "Failed to retrieve reviews from the server." });
        }

        public async Task<IActionResult> SubmitRating([FromBody] Ratings? rating)
        {
            string? token = "";
            string? phoneNumber = "";

            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            var identificationResult = await _authenticationManager.identificationValid();
            if (identificationResult != null)
            {
                phoneNumber = identificationResult?.UserData?.Code;
                token = identificationResult?.UserData?.Token;
            }
            if (rating != null)
            {
                rating.Code = phoneNumber;
            }
            var jsonBody = JsonConvert.SerializeObject(rating);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            if (rating == null) { return BadRequest("Error"); }
            else
            {
                _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage ratingResponse = await _v7Client.PostAsync($"review/save", content);
                if (ratingResponse.IsSuccessStatusCode)
                {
                    string responseData = await ratingResponse.Content.ReadAsStringAsync();
                    var review = JsonConvert.DeserializeObject<HulubejeResponse<bool>>(responseData);
                    return Json(review);
                }
                return BadRequest("Error");
            }

        }
        [Route("order")]
        public async Task<IActionResult> OrderDetail(string voucher)
        {
            try {
                string decryptedVoucher = await Decrypt(voucher);
                var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
                var identificationResult = await _authenticationManager.identificationValid();
                if (identificationResult != null)
                {
                    ViewBag.isVaild = identificationResult?.isValid;
                    ViewBag.isLoggedIn = identificationResult?.isLoggedIn;
                    ViewBag.FirstName = identificationResult?.UserData.FirstName;
                    ViewBag.LastName = identificationResult?.UserData.LastName;
                    ViewBag.MiddleName = identificationResult?.UserData.MiddleName;
                    ViewBag.Personalattachment = identificationResult?.UserData.PersonalAttachment;
                    ViewBag.Idnumber = identificationResult?.UserData.IdNumber;
                    ViewBag.Idtype = identificationResult?.UserData.IdType;
                    ViewBag.Dob = identificationResult?.UserData.Dob;
                    ViewBag.Idattachment = identificationResult?.UserData.IdAttachment;
                    ViewBag.PhoneNumber = identificationResult?.UserData.Code;
                    ViewBag.EmailAddress = identificationResult?.UserData.Email;
                }

                var review = new GetHistoryDetailResposne();
                HttpResponseMessage gethistorydetailResponse = await _v7Client.GetAsync($"voucher/gethistorydetailbyid?" +
                    $"voucherId={decryptedVoucher}");
                if (gethistorydetailResponse.IsSuccessStatusCode)
                {
                    string responseData = await gethistorydetailResponse.Content.ReadAsStringAsync();
                    review = responseData != null ? JsonConvert.DeserializeObject<GetHistoryDetailResposne>(responseData) : new GetHistoryDetailResposne();
                }
                var qrCodeBytes = Array.Empty<byte>();
                if (!string.IsNullOrWhiteSpace(review?.Data?.ExtraData?.Status))
                {
                    var text = $"CNET_REDEEM,{review?.Data?.ExtraData?.Tin},{review?.Data?.BranchCode?.ToString()},{review?.Data?.PhoneNumber}," +
                    $"{review?.Data?.ExtraData?.VoucherId},{review?.Data?.IssuedDate?.ToString("MM/dd/yyyy hh:mm:ss tt")}," +
                    $"{review?.Data?.GrandTotal?.ToString("0.00")}";
                    var encrypt = Encrypt(text, "MAKV2SPBNI992121");
                    qrCodeBytes = _qrCodeGeneratorService.GenerateQRCode(encrypt);
                }
                


                if (review != null & review?.Data?.ExtraData?.Status == "Reedemed")
                {
                    string overlayImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Assets", "redeemed_icon.jpg");
                    byte[] overlayImageBytes = System.IO.File.ReadAllBytes(overlayImagePath);

                    byte[] overlaidImageBytes = OverlayImages(qrCodeBytes, overlayImageBytes);

                    // Convert the overlaid image to a base64 string
                    var base64OverlaidImage = Convert.ToBase64String(overlaidImageBytes);
                    review.QRCodeImage = $"data:image/png;base64,{base64OverlaidImage}";
                }
                else
                {
                    var base64QRCode = Convert.ToBase64String(qrCodeBytes);
                    review.QRCodeImage = $"data:image/png;base64,{base64QRCode}";
                }
                return View(review);
            }
            catch
            {
                return View();
            }
            
        }

        [Route("print")]
        public async Task<IActionResult> VoucherLineItemPrintViewer(string voucher)
        
        {
            try 
            {
                string decryptedVoucher  = voucher;
                bool isEinvoice = false;
                if (!string.IsNullOrEmpty(voucher) && voucher.Contains("einvoice"))
                {
                    isEinvoice = true;
                    decryptedVoucher = decryptedVoucher.Replace("einvoice", "id");
                }
                else
                {
                    decryptedVoucher = await Decrypt(voucher?.ToString());
                }
                var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
                var eInvoiceClient = _httpClientFactory.CreateClient();

                var identificationResult = await _authenticationManager.identificationValid();
                var printvoucher = new HulubejeResponse<VoucherPrintModel>();
                if (isEinvoice)
                {
                    var splited = decryptedVoucher.Split("id");
                    var voucherId = splited.LastOrDefault();
                    var id = splited.FirstOrDefault();

                    string eInvoiceBaseUrl = await GetEInvoiceBaseUrl(id);

                    // 2️⃣ Configure HttpClient safely
                    eInvoiceClient.BaseAddress = new Uri(eInvoiceBaseUrl);

                    //HttpResponseMessage einvoiceResponse = await eInvoiceClient.GetAsync($"TransactionLibrary/get_einvoice_voucher_detail?voucherId={decryptedVoucher.Split("id").Last()}");
                    //if (einvoiceResponse.IsSuccessStatusCode)
                    //{
                    _httpContextAccessor?.HttpContext?
                    .Session
                    .SetString("EInvoiceBaseUrl", eInvoiceBaseUrl);
                    var _sharedHelpers = new SharedHelpers(eInvoiceClient, _httpContextAccessor!);
                    await _Initialization(eInvoiceClient);
                    var req = await _sharedHelpers.GetReqAsync<ResponseModel<VoucherDetailDTO>>($"TransactionLibrary/get_voucher_detail?voucherId={voucherId}");
                    
                    //SystemConstantDTO check_islineitem = GeneralBufferHolder.SystemConstants.FirstOrDefault(s => s.Id == VoucherDetail.VoucherHeader.DefinitionId);
                    //string einvoiceResponseData = await einvoiceResponse.Content.ReadAsStringAsync();
                    //var einvoiceDetails = einvoiceResponseData != null ? JsonConvert.DeserializeObject<ResponseModel<EinvoiceVoucherDetailDTO>>(einvoiceResponseData) : new ResponseModel<EinvoiceVoucherDetailDTO>();

                    //if (VoucherDetail != null && (check_islineitem?.Category == "LineItem" && (VoucherDetail.VoucherHeader?.DefinitionId == 197 || VoucherDetail.VoucherHeader?.DefinitionId == 217)))
                    //{
                    var printDocumentVoucher = new PrintDocumentVoucher(eInvoiceClient, _sharedHelpers, _ftpSettings);
                    printvoucher.Data = await printDocumentVoucher.EInvoiceLineItemVoucher(req?.Data ?? new VoucherDetailDTO());
                    printvoucher.Data.PaperSize = "A4";
                    var serializejson = JsonConvert.SerializeObject(printvoucher.Data);
                        //}
                        //else if (VoucherDetail != null && check_islineitem?.Category == "Non-LineItem")
                        //    return await printNonLineItemVoucher(VoucherDetail, isNotPartial);
                    //}
                }
                else
                {
                    HttpResponseMessage printvoucherResponse = await _v7Client.GetAsync($"voucher/printvoucher?VoucherId={decryptedVoucher}");
                    if (printvoucherResponse.IsSuccessStatusCode)
                    {
                        string responseData = await printvoucherResponse.Content.ReadAsStringAsync();
                        printvoucher = responseData != null ? JsonConvert.DeserializeObject<HulubejeResponse<VoucherPrintModel>>(responseData) : new HulubejeResponse<VoucherPrintModel>();
                    }
                }
                return View(printvoucher?.Data ?? new VoucherPrintModel());
            }
            catch (Exception ex) 
            {
                return View(new VoucherPrintModel());
            }
            
        }

        private async Task<string> GetEInvoiceBaseUrl(string? id)
        {
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");

            var response = await _v7Client.GetAsync($"voucher/getbaseurl?id={id}");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to retrieve E-Invoice base URL.");

            var content = await response.Content.ReadAsStringAsync();

            var baseUrl =
                JsonConvert.DeserializeObject<HulubejeResponse<string>>(content);

            if (baseUrl?.IsSuccessful != true ||
                string.IsNullOrWhiteSpace(baseUrl.Data))
                throw new Exception("Invalid E-Invoice base URL returned.");

            return $"{baseUrl.Data}/api/";
        }
        private async Task _Initialization(HttpClient client)
        {
            var _initialBufferPopulator = new InitialBufferPopulator(client);
            var _sharedHelpers = new SharedHelpers(client, _httpContextAccessor);
            var comp= await _sharedHelpers.GetCompany();
            GeneralBufferHolder.AllSubCountry = await _initialBufferPopulator.GetAllSubCountry();
            GeneralBufferHolder.CompanyInformations = await _sharedHelpers.GetCompanyInfo(comp?.Tin);
            GeneralBufferHolder.AllCurrencies = await _initialBufferPopulator.GetAllCurrencies();
            GeneralBufferHolder.SystemConstants = await _initialBufferPopulator.GetAllSystemConstants();
            GeneralBufferHolder.AllTaxs = await _initialBufferPopulator.GetAllTaxs();
            GeneralBufferHolder.ConfigrationSettings = await _initialBufferPopulator.GetAllConfigration();
            GeneralBufferHolder.AllActivityDefns = await _initialBufferPopulator.GetAllActivityDefns();
            GeneralBufferHolder.AllPeriods = await _initialBufferPopulator.GetPeriod();
            GeneralBufferHolder.AllPreferences = await _initialBufferPopulator.GetAllPreferences();
            GeneralBufferHolder.AllLookups = await _initialBufferPopulator.GetLookUps();
            GeneralBufferHolder.AllValueFactorDefinitions = await _initialBufferPopulator.GetAllValueFactorDefinitions();
            GeneralBufferHolder.AllRelationStates = await _initialBufferPopulator.GetAllRelationStates();
            GeneralBufferHolder.AllVoucherExtDefinitions = await _initialBufferPopulator.GetAllVoucherExtDefns();
            GeneralBufferHolder.AllConsineeUnit = await _initialBufferPopulator.GetAllConsigneeUnits(comp.Id);
            GeneralBufferHolder.AllVoucherTermDefinitions = await _initialBufferPopulator.GetAllVoucherTermDefns();
            GeneralBufferHolder.AllRelations = await _initialBufferPopulator.GetAllRelationss();
            GeneralBufferHolder.AllTermDefns = await _initialBufferPopulator.GetAllTermDefns();
            GeneralBufferHolder.AccountMapBufferList = await _initialBufferPopulator.GetAllAccountMap();
            GeneralBufferHolder.AllSerialDefinitions = await _initialBufferPopulator.GetAllSerialDefns();
            GeneralBufferHolder.AllUserRoleMapper = await _initialBufferPopulator.GetAllUserRoleMapper();
            GeneralBufferHolder.AllDevices = await _initialBufferPopulator.GetAllDevices();
            GeneralBufferHolder.AllFixedAssetArt = await _initialBufferPopulator.GetAllFixedAsset();
            GeneralBufferHolder.AllMappedStores = await _initialBufferPopulator.GetAllMappedStores();
        }
        //public static string ReplaceSpacesWithPlus(string input)
        //{
        //    return input.Replace(" ", "+");
        //}
        [Route("orderdetail")]
        public async Task<IActionResult> OrderDetail([FromQuery] VoucherData? voucherData)
        {
            try
            {
                string? token = "";
                string? phoneNumber = "";

                var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
                var identificationResult = await _authenticationManager.identificationValid();
                if (identificationResult != null)
                {
                    phoneNumber = identificationResult?.UserData?.Code;
                    token = identificationResult?.UserData?.Token;
                    ViewBag.isVaild = identificationResult?.isValid;
                    ViewBag.isLoggedIn = identificationResult?.isLoggedIn;
                    ViewBag.FirstName = identificationResult?.UserData.FirstName;
                    ViewBag.LastName = identificationResult?.UserData.LastName;
                    ViewBag.MiddleName = identificationResult?.UserData.MiddleName;
                    ViewBag.Personalattachment = identificationResult?.UserData.PersonalAttachment;
                    ViewBag.Idnumber = identificationResult?.UserData.IdNumber;
                    ViewBag.Idtype = identificationResult?.UserData.IdType;
                    ViewBag.Dob = identificationResult?.UserData.Dob;
                    ViewBag.Idattachment = identificationResult?.UserData.IdAttachment;
                    ViewBag.PhoneNumber = identificationResult?.UserData.Code;
                    ViewBag.EmailAddress = identificationResult?.UserData.Email;
                }

                var review = new GetHistoryDetailResposne();
                _v7Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage gethistorydetailResponse = await _v7Client.GetAsync($"voucher/gethistorydetail?" +
                    $"voucherCode={voucherData?.VoucherCode}&companyCode={voucherData?.CompanyCode}&industryType={voucherData?.IndustryType}");
                if (gethistorydetailResponse.IsSuccessStatusCode)
                {
                    string responseData = await gethistorydetailResponse.Content.ReadAsStringAsync();
                    review = responseData != null ? JsonConvert.DeserializeObject<GetHistoryDetailResposne>(responseData) : new GetHistoryDetailResposne();
                }
                var qrCodeBytes = Array.Empty<byte>();

                if (review?.Data?.ExtraInformation?.Count != 0 || review?.Data?.ExtraData?.Status != null)
                {
                    var text = $"CNET_REDEEM,{review?.Data?.ExtraData?.Tin},{voucherData?.BranchCode},{phoneNumber}," +
                    $"{review?.Data?.ExtraData?.VoucherId},{voucherData?.IssuedDate.ToString("MM/dd/yyyy hh:mm:ss tt")}," +
                    $"{review?.Data?.GrandTotal?.ToString("0.00")}";
                    var encrypt = Encrypt(text, "MAKV2SPBNI992121");
                    qrCodeBytes = _qrCodeGeneratorService.GenerateQRCode(encrypt);
                }




                if (review != null & review?.Data?.ExtraData?.Status == "Reedemed" && review?.Data!=null)
                {
                    review.Data.CompanyName = voucherData?.CompanyName;
                    review.Data.VoucherCode = voucherData?.VoucherCode;
                    review.Data.IssuedDate = voucherData?.IssuedDate;
                    string overlayImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Assets", "redeemed_icon.jpg");
                    byte[] overlayImageBytes = System.IO.File.ReadAllBytes(overlayImagePath);

                    byte[] overlaidImageBytes = OverlayImages(qrCodeBytes, overlayImageBytes);

                    // Convert the overlaid image to a base64 string
                    var base64OverlaidImage = Convert.ToBase64String(overlaidImageBytes);
                    review.QRCodeImage = $"data:image/png;base64,{base64OverlaidImage}";
                }
                else
                {
                    var base64QRCode = Convert.ToBase64String(qrCodeBytes);
                    review.QRCodeImage = $"data:image/png;base64,{base64QRCode}";
                }
                review.VoucherData = voucherData;
                return View(review);
            }
            catch (Exception ex) 
            {
                return View(null);
            }
           
        }

        private static byte[] OverlayImages(byte[] baseImageBytes, byte[] overlayImageBytes)
        {
            // Load base image (QR code)
            using (MemoryStream baseStream = new MemoryStream(baseImageBytes))
            using (Image baseImage = Image.FromStream(baseStream))
            {
                // Load overlay image and resize it
                using (MemoryStream overlayStream = new MemoryStream(overlayImageBytes))
                using (Image overlayImage = Image.FromStream(overlayStream))
                {
                    // Calculate desired dimensions for the overlay image (covering more of the base image)
                    int overlayWidth = baseImage.Width;
                    int overlayHeight = overlayWidth * overlayImage.Height / overlayImage.Width; // Maintain aspect ratio

                    // Resize overlay image
                    Image resizedOverlay = new Bitmap(overlayImage, new Size(overlayWidth, overlayHeight));

                    // Create a new bitmap with the same dimensions as the base image
                    using (Bitmap resultImage = new Bitmap(baseImage.Width, baseImage.Height))
                    {
                        // Use Graphics to draw both images onto the result image
                        using (Graphics graphics = Graphics.FromImage(resultImage))
                        {
                            // Draw base image (QR code)
                            graphics.DrawImage(baseImage, Point.Empty);

                            // Calculate position to place overlay image diagonally from bottom left to top right
                            int posX = baseImage.Width - overlayWidth; // Move further to the right
                            int posY = baseImage.Height - overlayHeight * 2; // Move upwards more

                            // Translate and rotate Graphics object
                            graphics.TranslateTransform(posX + resizedOverlay.Width / 2, posY + resizedOverlay.Height / 2);
                            graphics.RotateTransform(-45); // Rotate around the center of the overlay

                            // Draw resized overlay image
                            graphics.DrawImage(resizedOverlay, -resizedOverlay.Width / 2, -resizedOverlay.Height / 2);

                            // Reset transformations (optional, depending on subsequent operations)
                            graphics.ResetTransform();
                        }

                        // Convert the composite image back to bytes (PNG format for transparency)
                        using (MemoryStream resultStream = new MemoryStream())
                        {
                            resultImage.Save(resultStream, ImageFormat.Png);
                            return resultStream.ToArray();
                        }
                    }
                }
            }
        }





        public static string Encrypt(string plainText, string password)
        {
            byte[] encryptedBytes;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(password);
                aesAlg.IV = Encoding.UTF8.GetBytes(password);

                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Mode = CipherMode.CBC;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using MemoryStream msEncrypt = new();
                using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
                using (StreamWriter swEncrypt = new(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }
                encryptedBytes = msEncrypt.ToArray();
            }
            return Convert.ToBase64String(encryptedBytes);
        }


        public async Task<string> Decrypt(string? cipherText)
        {
            var _v7Client = _httpClientFactory.CreateClient("HulubejeBooking");
            HttpResponseMessage gethistorydetailResponse = await _v7Client.GetAsync($"decrypt?encryptedText={cipherText}");
            if (gethistorydetailResponse.IsSuccessStatusCode)
            {
                string plaintext = await gethistorydetailResponse.Content.ReadAsStringAsync();

                return plaintext;
            }
            return "";

        }

        public IActionResult GenerateQRCode(string text)
        {
            byte[] qrCodeBytes = _qrCodeGeneratorService.GenerateQRCode(text);
            return File(qrCodeBytes, "image/png");
        }
    }
}
