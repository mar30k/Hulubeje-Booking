using HulubejeBooking.Models.CInemaModels;

namespace HulubejeBooking.Models.PaymentModels
{
    public class Seats
    {
        public int? Schedule { get; set; }
        public int? Seat { get; set; }
    }

    public class CacheDetail
    {
        public string? Key { get; set; }
        public string? ExtensionDeligate { get; set; }
        public int? Extension { get; set; }
    }

    public class Movie
    {
        public int? Article { get; set; }
        public List<Seats>? Seats { get; set; }
        public CacheDetail? CacheDetail { get; set; }
    }

    public class LineItem
    {
        public string? Name { get; set; }
        public int? Article { get; set; }
        public decimal? UnitAmount { get; set; }
        public int? Quantity { get; set; }
        public int? Uom { get; set; } = 0;
        public string? SpecialFlag { get; set; }
    }

    public class PaymentTransactionRequest
    {
        public string? UserMobileNumber { get; set; }
        public string? PaymentProcessorUnitName { get; set; }
        public int? OperationMode { get; set; }
        public int? SupplierConsigneeId { get; set; }
        public int? SupplierConsigneeUnit { get; set; }
        public int? PaymentProcessorConsigneeId { get; set; }
        public int? PaymentProcessorConsigneeUnit { get; set; }
        public string? TransactionId { get; set; }
        public decimal? Amount { get; set; }
        public ReferenceNo? AdditionalParameters { get; set; }
        public string? Pin { get; set; }
    }
    public class ReferenceNo
    {
        public string? ReferenceNumber { get; set; }
    }
    public class PaymentInfo
    {
        public string? Type { get; set; }
        public string? IsAsyncMode { get; set; }
        public PaymentTransactionRequest? PaymentTransactionRequest { get; set; }
    }

    public class ActivityLog
    {
        public string? Code { get; set; }
        public string? Target { get; set; } = "";
        public string? Platform { get; set; } = "Web";
        public int Latitude { get; set; } = 0;
        public int Longitude { get; set; } = 0;
        public string? AppVersion { get; set; } = "";
    }
    public class Guest
    {
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNo { get; set; }
        public string? IdType { get; set; }
        public string? IdNumber { get; set; }
        public int Nationality { get; set; } = 1;
        public string? Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Gender { get; set; }
    }

    public class HotelDetail
    {
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public int RoomTypeCode { get; set; }
        public int RateCode { get; set; }
        public int RateCodeDetail { get; set; }
        public double TotalAmount { get; set; }
        public double AverageAmount { get; set; }
        public List<Guest>? Guests { get; set; }
        public int RoomCount { get; set; }
        public string? SpecialRequirement { get; set; }
        public string? CashReceiptVoucher { get; set; }
    }
    public class PaymentModel
    {
        public decimal? Amount { get; set; }
        public string? Code { get; set; }
        public int? CompanyCode { get; set; }
        public int? BranchCode { get; set; }
        public int? IndustryType { get; set; }
        public List<LineItem>? LineItems { get; set; }
        public string? PaymentMethod { get; set; }
        public string? TransactionReference { get; set; }
        public PaymentInfo? PaymentInfo { get; set; }
        public int Latitude { get; set; } = 0;
        public int Longitude { get; set; } = 0;
        public string Platform { get; set; } = "Web";
        public Movie? Movie { get; set; }
        public HotelDetail? HotelDetail { get; set; }
        public ActivityLog? ActivityLog { get; set; } 
    }

}
