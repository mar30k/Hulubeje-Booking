using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
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
                claims.Add(new Claim(ClaimTypes.Name, user.phoneNumber, ClaimValueTypes.String, "cnetERP"));


            //create principal for the current authentication scheme
            var userIdentity = new ClaimsIdentity(claims, "cnet.erp.v6");
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            //set value indicating whether session is persisted and the time at which the authentication was issued
            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                IssuedUtc = DateTime.UtcNow
            };

            //sign in
            await _httpContextAccessor.HttpContext.SignInAsync("cnet.erp.v6", userPrincipal, authenticationProperties);

            //cache authenticated customer
            _cachedUser = user;
        }
        public virtual async Task<cookieValidation> identificationValid()
        {
            var validinfo = new cookieValidation();

            var authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();

            if (authenticateResult.Succeeded)
            {
                var authenticationProperties = authenticateResult.Properties;

                if (authenticationProperties?.IsPersistent == true)
                {
                    validinfo.isValid = true;  // User is authenticated and the authentication is persistent
                    return validinfo;
                }
            }

            validinfo.isValid = false;
            return validinfo;
        }

        public virtual async void SignOut()
        {
            //reset cached customer
            _cachedUser = null;
            //and sign out from the current authentication scheme
            await _httpContextAccessor.HttpContext.SignOutAsync("cnet.erp.v6");
        }
    }
}