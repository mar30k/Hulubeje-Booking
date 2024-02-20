namespace HulubejeBooking.Models.Authentication
{
    public class ChangePasswordModel
    {
        public string? RepeatPassword { get; set; }
        public string? Oldpassword { get; set; }
        public string? Newpassword { get; set; }
    }
}
