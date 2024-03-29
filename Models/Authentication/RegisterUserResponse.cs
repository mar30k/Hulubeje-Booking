namespace HulubejeBooking.Models.Authentication
{
    public class RegisterUserResponse
    {
        public bool IsSuccessful { get; set; }
        public UserData? Data { get; set; }
        public List<string>? ErrorMessages { get; set; }
        public List<string>? AdditionalParameters { get; set; }
    }

}
