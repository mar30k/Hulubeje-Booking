using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HulubejeBooking.Models.Authentication
{
    public class PersonModel
    {
        public string? PersonCode { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Middle Name is required")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date of Birth")]
        public string? Dob { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string? Password { get; set; }

        public string? ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        public string? Gender { get; set; }

        public string? EmailAddress { get; set; }
        public string? IdType { get; set; }
        public string? Id { get; set; }
        public string? IdPhoto { get; set; }
        public string? PersonalPhoto { get; set; }

    }
    public class UserInformation
    {
        public string? code { get; set; }
        public string? firstName { get; set; }
        public string? middleName { get; set; }
        public int? successCode { get; set; }
        public string? lastName { get; set; }
        public string? phoneNumber { get; set; }
        public string? emailAddress { get; set; }
        public string? gender { get; set; }
        public string? idtype { get; set; }
        public string? idnumber { get; set; }
        public string? dob { get; set; }
        public string? idattachment { get; set; }
        public string? personalattachment { get; set; }
    }

    public class UserResponse
    {
        public int? statusCode { get; set; }
        public UserInformation? userInformation { get; set; }
    }
    public class MessageResponse
    {
        public bool? isSent { get; set; }
        public string? messageId { get; set; }
        public string? to { get; set; }
        public string? code { get; set; }
        public string? verificationId { get; set; }
        public List<string?>? errors { get; set; }
    }
    public class VerifyResponse
    {
        public bool? isVerified { get; set; }
        public List<string?>? errors { get; set; }
    }
    public class LoginInformation
    {

        [Required(ErrorMessage = "Password is required!")]
        [DataType(DataType.Password)]
        //[NoTrim]
        [DisplayName("Password")]
        public string? Password { get; set; }
        [Required(ErrorMessage = "Phone Number is required!")]
        [DataType(DataType.Text)]
        [DisplayName("Phone Number")]
        public string? Phone { get; set; }
        [DisplayName("Remember Me")]
        public bool RememberMe { get; set; }
    }
    public class cookieValidation
    {
        public bool isValid { get; set; }
        public bool isLoggedIn { get; set; }   
    }
}
