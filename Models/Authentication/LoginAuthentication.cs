namespace HulubejeBooking.Models.Authentication
{

    public class LoginAuthentication
    {
        public bool IsSuccessful { get; set; }
        public UserData? Data { get; set; }
        public string[]? ErrorMessages { get; set; }
        public object? AdditionalParameters { get; set; }
    }

    public class UserData
    {
        public string? Code { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public string? Occupation { get; set; }
        public DateTime? Dob { get; set; }
        public string? Nationality { get; set; }
        public string? IdType { get; set; }
        public string? IdNumber { get; set; }
        public string? IdAttachment { get; set; }
        public string? PersonalAttachment { get; set; }
        public string? Token { get; set; }
        public string? Pivk { get; set; }
    }
}