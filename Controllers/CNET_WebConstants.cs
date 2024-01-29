namespace HulubejeBooking.Controllers
{
    public class CNET_WebConstants
    {
        public static string ClaimsIssuer => "cnetERP";
        public static string CookieScheme => "cnet.erp.v6";
        public static string IdentificationCookie => "cnet.erp.v6.myId";

        public const int IdentificationCookieLifeTime = 10080;
        public const int IdentificationCookieDailyLifeTime = 1440;
    }
}
