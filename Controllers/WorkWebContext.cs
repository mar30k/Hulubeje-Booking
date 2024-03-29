using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.Authentication;

namespace HulubejeBooking.Controllers
{
    public class WorkWebContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private UserData _cachedCustomer;
        private AuthenticationManager _authenticationManager;
        private IBuffer _buffer;
        public WorkWebContext(AuthenticationManager authenticationManager, Buffers buffer, IHttpContextAccessor httpContextAccessor)
        {
            _authenticationManager = authenticationManager;
            _buffer = buffer;
            _httpContextAccessor = httpContextAccessor;
        }

        //protected virtual void SetCustomerCookie(string consignee)
        //{
        //    if (_httpContextAccessor.HttpContext?.Response?.HasStarted ?? true)
        //        return;

        //    //delete current cookie value
        //    var cookieName = $"{CNET_WebConstants.Prefix}{NopCookieDefaults.CustomerCookie}";
        //    _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookieName);

        //    //get date of cookie expiration
        //    var cookieExpires = _cookieSettings.CustomerCookieExpires;
        //    var cookieExpiresDate = DateTime.Now.AddHours(cookieExpires);

        //    //if passed consignee is empty set cookie as expired
        //    if (consignee == null)
        //        cookieExpiresDate = DateTime.Now.AddMonths(-1);

        //    //set new cookie value
        //    var options = new CookieOptions
        //    {
        //        HttpOnly = true,
        //        Secure = true,
        //    };

        //    _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, consignee, options);
        //}

        public virtual async Task<UserData> GetCurrentCustomerAsync()
        {
            _cachedCustomer = await _buffer.GetCurrentCustomerFromBuffer();

            //whether there is a cached value
            if (_cachedCustomer != null)
                return _cachedCustomer;


            await SetCurrentCustomerAsync();

            return _cachedCustomer;
        }

        /// <summary>
        /// Sets the current customer
        /// </summary>
        /// <param name="customer">Current customer</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SetCurrentCustomerAsync(UserData customer = null)
        {
            if (customer == null)
            {
                var user = await _authenticationManager.GetAuthenticatedCustomerAsync();

                if (user == null)
                {
                    var currentUser = await _authenticationManager.GetOrCreateBackgroundTaskUserAsync();

                    _cachedCustomer = currentUser;
                }

                else
                {
                    _cachedCustomer = user;
                }
            }

            else
            {
                //SetCustomerCookie(customer.Token);
                _cachedCustomer = customer;
            }

            _buffer.AddCustomerToBuffer(_cachedCustomer);
        }
        
    }
}
