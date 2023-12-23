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

    public class PaymentInfoModel
    {
        public string UserAccessToken { get; set; } = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYmYiOjE2NzM4NTczMzksImV4cCI6MTY3MzkwMDUzOSwiaWF0IjoxNjczODU3MzM5LCJpc3MiOiJDTkVULlBheW1lbnQuQVBJIiwiYXVkIjoiQ05FVC5QYXltZW50LkFQSSJ9.Hh_CF39I-tk11-SJeDzjXhq7o8ka4_cAIsGklOyzgKU";

        public PaymentTransactionRequestModel PaymentTransactionRequest { get; set; }

        public bool IsAsyncMode { get; set; } = true;
    }
    public class PaymentTransactionRequestModel
    {
        public string UserMobileNumber { get; set; } 
        public string SupplierTin { get; set; } = "0054889410";
        public string SupplierOUD { get; set; } = "OUD100007845";
        public string TransactionId { get; set; } = "0939977886-107836-720";
        public string Amount { get; set; } 
        public string PaymentProviderOUD { get; set; }
        public string Pin { get; set; } = "785686";
        public string ExpirationDate { get; set; } = "";
    }


}
