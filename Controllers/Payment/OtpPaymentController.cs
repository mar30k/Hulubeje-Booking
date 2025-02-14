﻿using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.PaymentModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace HulubejeBooking.Controllers.Payment
{
    public class OtpPaymentController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;
        private readonly AuthenticationManager _authenticationManager;
        public OtpPaymentController(IHttpClientFactory httpClientFactory, IHttpContextAccessor? httpContextAccessor, AuthenticationManager authenticationManager)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var paymentOptionsJson = HttpContext.Session.GetString("PaymentOptions");
            var value = HttpContext.Session.GetString("cinema");
            if (value != null)
            {
                ViewBag.CoutDown = value;
            }
            var errorValue = HttpContext.Session.GetString("error");
            if (errorValue != null)
            {
                TempData["ErrorMessage"] = "Incorrect Otp Or Couldn't Process Your Payment!";
            }
            //HttpContext.Session.Remove("error");
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
            }

            PaymentTransactionRequest? paymentntTranscion;
            if (HttpContext.Session.TryGetValue("PaymentTransactionRequest", out var paymentTransactionBytes))
            {
                var paymentTransactionResponse = Encoding.UTF8.GetString(paymentTransactionBytes);
                paymentntTranscion = JsonConvert.DeserializeObject<PaymentTransactionRequest>(paymentTransactionResponse);
            }
            else
            {
                TempData["ErrorMessage"] = "Session Has Expired Please Restart the Booking Process";
                return RedirectToAction("Index", "Home");
            }
            return View(paymentntTranscion);
        }
    }
}
