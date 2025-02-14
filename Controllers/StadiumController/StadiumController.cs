﻿using HulubejeBooking.Controllers.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HulubejeBooking.Controllers.StadiumController
{
    public class StadiumController : Controller
    {
        private IHttpContextAccessor? _httpContextAccessor;
        private AuthenticationManager _authenticationManager;

        public StadiumController(IHttpContextAccessor? httpContextAccessor, AuthenticationManager authenticationManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
        }
        public async Task<IActionResult> IndexAsync()
        {
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
            return View();
        }
    }
}
