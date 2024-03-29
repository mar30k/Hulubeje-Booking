using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using HulubejeBooking.Controllers;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using HulubejeBooking.Models.Authentication;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Text;
using System.Net.Http.Headers;
using HulubejeBooking.Models.HotelModels;
using System;
using System.Diagnostics;
using NuGet.Common;
using System.Data;
using System.Net.Mail;
using HulubejeBooking.Models.CInemaModels;

namespace HulubejeBooking.Controllers.Authentication
{
    public class AuthenticationManager
    {
        private IHttpContextAccessor _httpContextAccessor;
        private HotelListBuffer _hotelListBuffer;
        private IBuffer _buffer;
        private UserData? _cachedUser;
        private IHttpClientFactory _httpClientFactory;
        //private WorkWebContext _workWebContext;
        public AuthenticationManager(
                IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory, HotelListBuffer hotelListBuffer, Buffers buffer
                )
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _hotelListBuffer = hotelListBuffer;
            _buffer = buffer;
        }


        
        public virtual async void SignIn(UserData user, bool isPersistent)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            //create claims for customer's username and email
            var claims = new List<Claim>();

            if (!string.IsNullOrEmpty(user.Token))
                claims.Add(new Claim(ClaimTypes.Name, user.Token, ClaimValueTypes.String, CNET_WebConstants.ClaimsIssuer));


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

            _cachedUser = user;
            _buffer.AddCustomerToBuffer(_cachedUser);
        }
        public virtual async Task<cookieValidation> identificationValid()
        {
            var validinfo = new cookieValidation();
            var data = new HulubejeBooking.Models.Authentication.UserData(); // Adjust the namespace here
            var loggedInCheckerJson = _httpContextAccessor?.HttpContext?.Session.GetString("isLoggedIn");
            var loginChecker = loggedInCheckerJson != null ? JsonConvert.DeserializeObject<bool>(loggedInCheckerJson) : false;
            var authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();

            if (authenticateResult.Succeeded)
            {
                var usernameClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name
                    && claim.Issuer.Equals(CNET_WebConstants.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase));
                var token = usernameClaim?.Value.ToString();
                if (!string.IsNullOrEmpty(token))
                {
                    data = await _buffer.GetCurrentCustomerFromBuffer();
                }
                var authenticationProperties = authenticateResult.Properties;

                if (authenticationProperties?.IsPersistent == true)
                {
                    var userDatas = data;
                    validinfo.UserData = userDatas;
                    validinfo.isValid = true;
                    validinfo.isLoggedIn = loginChecker;

                    return validinfo;
                }
                else
                {
                    var userDatas = data;
                    validinfo.UserData = userDatas;
                    validinfo.isValid = true;
                    validinfo.isLoggedIn = loginChecker;

                    return validinfo;
                }
            }
            else
            {
                var guestUser = GetOrCreateBackgroundTaskUserAsync();
                validinfo.UserData = guestUser.Result;
                validinfo.isValid = false;
                validinfo.isLoggedIn = loginChecker;

                return validinfo;
            }            
        }




        public async Task<UserData> GetUserData(string token)
        {
            var _V7client = _httpClientFactory.CreateClient("HulubejeBooking");
            var param = new { };
            var jsonBody = JsonConvert.SerializeObject(param);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            _V7client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var authentication = await _V7client.PostAsync("auth/loginwithtoken", content);
            if (authentication.IsSuccessStatusCode)
            {
                string userDataString = await authentication.Content.ReadAsStringAsync();
                var userDatas = JsonConvert.DeserializeObject<HulubejeBooking.Models.Authentication.LoginAuthentication>(userDataString);
                _cachedUser = userDatas?.Data;
            }
            return _cachedUser;
        }
        public async Task<UserData> GetOrCreateBackgroundTaskUserAsync()
        {
            var loginresponseData = new LoginAuthentication();
            var _V7client = _httpClientFactory.CreateClient("HulubejeBooking");
            var param = new
            {
                code = "0000000000",
                password = "0000000000",
                isChangePassword = false
            };
            var jsonRequest = JsonConvert.SerializeObject(param);

            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            HttpResponseMessage loginResponse = await _V7client.PostAsync("auth/login", content);
            if (loginResponse.IsSuccessStatusCode)
            {
                var loginData = await loginResponse.Content.ReadAsStringAsync();
                loginresponseData = JsonConvert.DeserializeObject<LoginAuthentication>(loginData);
            }
            _cachedUser = loginresponseData?.Data;
            return _cachedUser;
        }
        public virtual async void SignOut()
        {
            _cachedUser = null;
            _buffer.DeleteCurrentCustomerFromBuffer();
            await _httpContextAccessor.HttpContext.SignOutAsync(CNET_WebConstants.CookieScheme);
        }

        public virtual async Task<UserData> GetAuthenticatedCustomerAsync()
        {
            //whether there is a cached customer 
            if (_cachedUser != null)
                return _cachedUser;
            var authenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();
            var customer = new UserData(); // Adjust the namespace here
            if (!authenticateResult.Succeeded)
            {
                return null;
            }
            else
            {
                var usernameClaim = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name
                    && claim.Issuer.Equals(CNET_WebConstants.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase));
                var token = usernameClaim?.Value.ToString();
                if (!string.IsNullOrEmpty(token))
                {
                    customer = await GetUserData(token);
                }
                _cachedUser = customer !=null ? customer : null;
            }
            return _cachedUser;
        }
    }
}