using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace HulubejeBooking.Models.Authentication
{
    public class PersonModel
    {
        public string? personCode { get; set; }
        public string? firstName { get; set; }
        public string? middleName { get; set; }
        public string? lastName { get; set; }
        public string? phoneNumber { get; set; }
        public string? emailAddress { get; set; }
        public string? gender { get; set; }
        public string? dob { get; set; }
        public string? password { get; set; }
        public string? idType { get; set; }
        public string? id { get; set; }
        public string? idPhoto { get; set; }
        public string? personalPhoto { get; set; }
        public UserResponse? signUpAuth { get; set; }
        public MessageResponse? messageResponse { get; set; }

    }
    public class UserInformation
    {
        public string? code { get; set; }
        public string? firstName { get; set; }
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
        public List<string>? errors { get; set; }
    }
    public class VerifyResponse
    {
        public bool? isVerified { get; set; }
        public List<string>? errors { get; set; }
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
}
