using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using HulubejeBooking.Controllers;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using HulubejeBooking.Models.Authentication;

namespace HulubejeBooking.Controllers.Authentication
{
    public class AuthenticationManager
    {
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;

        private UserInformation _cachedUser;
        public AuthenticationManager(
                IHttpContextAccessor httpContextAccessor,
                IHttpClientFactory httpClientFactory
                )
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
        }


        
        public virtual async void SignIn(UserInformation user, bool isPersistent)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            //create claims for customer's username and email
            var claims = new List<Claim>();

            if (!string.IsNullOrEmpty(user.phoneNumber))
                claims.Add(new Claim(ClaimTypes.Name, user.phoneNumber, ClaimValueTypes.String, CNET_WebConstants.ClaimsIssuer));


            //create principal for the current authentication scheme
            var userIdentity = new ClaimsIdentity(claims, CNET_WebConstants.CookieScheme);
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            //set value indicating whether session is persisted and the time at which the authentication was issued
            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                IssuedUtc = DateTime.UtcNow
            };

            //sign in
            await _httpContextAccessor.HttpContext.SignInAsync(CNET_WebConstants.CookieScheme, userPrincipal, authenticationProperties);

            //cache authenticated customer
            _cachedUser = user;
        }
        public virtual async Task<cookieValidation> identificationValid()
        {
            var validinfo = new cookieValidation();
            var validation = new cookieValidation();

            var loginChecker = false;
            var loggedInCheckerJson = _httpContextAccessor.HttpContext.Session.GetString("isLoggedIn");
            if (loggedInCheckerJson != null) {
                string isLoggedInJson = _httpContextAccessor.HttpContext.Session.GetString("isLoggedIn");
                 loginChecker = JsonConvert.DeserializeObject<bool>(isLoggedInJson);

            }
            var authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();

            if (authenticateResult.Succeeded)
            {
                var authenticationProperties = authenticateResult.Properties;

                if (authenticationProperties?.IsPersistent == true)
                {
                    validinfo.isValid = true;
                    validation = new cookieValidation{
                        isValid = validinfo.isValid,
                        isLoggedIn = loginChecker,
                    };

                    return validation;
                }
            }

            validinfo.isValid = false;
             validation = new cookieValidation
            {
                isValid = validinfo.isValid,
                isLoggedIn = loginChecker,
            };

            return validation;
        }

        public virtual async void SignOut()
        {
            _cachedUser = null;
            await _httpContextAccessor.HttpContext.SignOutAsync(CNET_WebConstants.CookieScheme);
        }
    }
}