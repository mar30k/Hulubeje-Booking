using HulubejeBooking.Models;
using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.BusModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using HulubejeBooking.Controllers.Authentication;
using Tweetinvi.Core.Models;
using NuGet.Common;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Security.Cryptography;
using System.Drawing.Imaging;
using System.Drawing;
using CNET_ERP_V7_VoucherPrintDialogue.Models;
using HulubejeBooking.Models.EventModels;
namespace HulubejeBooking.Controllers
{
    public class HistoryController : Controller
    {
        private readonly IQRCodeGeneratorService _qrCodeGeneratorService;
        private readonly AuthenticationManager _authenticationManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;

        public HistoryController(IHttpClientFactory httpClientFactory, IHttpContextAccessor? httpContextAccessor, AuthenticationManager authenticationManager, IQRCodeGeneratorService qRCodeGeneratorService)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
            _qrCodeGeneratorService = qRCodeGeneratorService;
        }

        public async Task<IActionResult> IndexAsync(string? phoneNumber)
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
            HttpResponseMessage historyResponse = await _v7Client.GetAsync($"voucher/gethistory?code={phoneNumber}&page=1");
            if (historyResponse.IsSuccessStatusCode)
            {
                string responseData = await historyResponse.Content.ReadAsStringAsync();
                var history = JsonConvert.DeserializeObject<OrdersModel>(responseData);
                historyWrapper.OrdersModel = history;
            }

            return View(historyWrapper);

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
                rating.BranchCode = 45;
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
                    var review = JsonConvert.DeserializeObject<RatingResponse>(responseData);
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

                var text = $"CNET_REDEEM,{review?.Data?.ExtraData?.Tin},{review?.Data?.BranchCode?.ToString()},{review?.Data?.PhoneNumber}," +
                    $"{review?.Data?.ExtraData?.VoucherId},{review?.Data?.IssuedDate?.ToString("MM/dd/yyyy hh:mm:ss tt")}," +
                    $"{review?.Data?.GrandTotal?.ToString("0.00")}";
                var encrypt = Encrypt(text, "MAKV2SPBNI992121");
                var qrCodeBytes = _qrCodeGeneratorService.GenerateQRCode(encrypt);


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
                string decryptedVoucher = await Decrypt(voucher?.ToString());
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

                var printvoucher = new HulubejeResponse<VoucherPrintModel>();
                HttpResponseMessage printvoucherResponse = await _v7Client.GetAsync($"voucher/printvoucher?VoucherId={decryptedVoucher}");
                if (printvoucherResponse.IsSuccessStatusCode)
                {
                    string responseData = await printvoucherResponse.Content.ReadAsStringAsync();
                    printvoucher = responseData != null ? JsonConvert.DeserializeObject<HulubejeResponse<VoucherPrintModel>>(responseData) : new HulubejeResponse<VoucherPrintModel>();
                }
                return View(printvoucher?.Data ?? new VoucherPrintModel());
            }
            catch (Exception ex) 
            {
                return View(new VoucherPrintModel());
            }
            
        }
        
        //public static string ReplaceSpacesWithPlus(string input)
        //{
        //    return input.Replace(" ", "+");
        //}
        [Route("history/orderdetail")]
        public async Task<IActionResult> OrderDetail([FromForm] VoucherData? voucherData)
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

                var text = $"CNET_REDEEM,{review?.Data?.ExtraData?.Tin},{voucherData?.BranchCode},{phoneNumber}," +
                    $"{review?.Data?.ExtraData?.VoucherId},{voucherData?.IssuedDate.ToString("MM/dd/yyyy hh:mm:ss tt")}," +
                    $"{review?.Data?.GrandTotal?.ToString("0.00")}";
                var encrypt = Encrypt(text, "MAKV2SPBNI992121");
                var qrCodeBytes = _qrCodeGeneratorService.GenerateQRCode(encrypt);


                if (review != null & review?.Data?.ExtraData?.Status == "Reedemed")
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
