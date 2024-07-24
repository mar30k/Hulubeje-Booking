using HulubejeBooking.Models.PaymentModels;
namespace HulubejeBooking.Models.HotelModels
{
    public class Guests
    {
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string IdPicture { get; set; } 
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string ResidentId { get; set; }
    }
    public class GuestModelWrapper
    {
        public RoomType? RoomType { get; set; }
        public List<Guests>? Guests { get; set; }
        public RoomFormData? RoomFormData { get; set; }
        public string? SpecialRequirement { get; set; }
    }


}
