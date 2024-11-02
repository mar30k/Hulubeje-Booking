using HulubejeBooking.Controllers.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace HulubejeBooking.Controllers.SpaController
{
    public class Spabill : Controller
    {
        
        private readonly IHttpClientFactory _httpClientFactory;
        private IHttpContextAccessor? _httpContextAccessor;
        private AuthenticationManager _authenticationManager;
        public Spabill(IHttpContextAccessor? httpContextAccessor, AuthenticationManager authenticationManager, IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _authenticationManager = authenticationManager;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> IndexAsync(string phone, string name)
        {
            var token = "";
            var identificationResult = await _authenticationManager.identificationValid();
            if (identificationResult != null)
            {
                token = identificationResult.UserData.Token;
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
