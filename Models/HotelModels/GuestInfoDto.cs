using HulubejeBooking.Models.PaymentModels;
namespace HulubejeBooking.Models.HotelModels
{
    public class GuestInfoDto
    {
        public string orgTin { get; set; }
        public string voucherCode { get; set; } = "";
        public string arrivalDate { get; set; } 
        public string departureDate { get; set; } 
        public string adult { get; set; } 
        public string child { get; set; } 
        public string roomTypeCode { get; set; }
        public string rateCode { get; set; } 
        public string rateCodeDetail { get; set; } 
        public string averageAmount { get; set; } = "0.2";
        public string totalAmount { get; set; } 
        public string PaymentMethod { get; set; }
        public string RoomCount { get; set; } 
        public Guests guests { get; set; }
        public string oud { get; set; } 
        public string SpecialRequirement { get; set; } = "";
        public string CashReceiptVoucher { get; set; } = "0939977886-13467-196";
        public string TransactionReference { get; set; } = "";
        public HotelBookingSuccessModel onHotelBookingSuccess { get; set; }
        public PaymentInfoModel PaymentInfo { get; set; }

    }

    public class Guests
    {
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string IdPicture { get; set; } = "";
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
    }

    public class HotelBookingSuccessModel
    {
        public string firstName { get; set; } = "Nathnel";
        public string company { get; set; } = "Alfarag";
        public string branch { get; set; }
        public int nightCount { get; set; }
    }



}
