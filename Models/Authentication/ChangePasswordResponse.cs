namespace HulubejeBooking.Models.Authentication
{

    public class ChangePasswordResponse
    {
        public bool? IsSuccessful { get; set; }
        public bool Data { get; set; }
        public List<string>? ErrorMessages { get; set; }
        public List<string>? AdditionalParameters { get; set; }
    }

}
