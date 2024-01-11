
namespace HulubejeBooking.Models.PaymentModels.HotlePaymentModels
{
    public class GuestInfoModel
    {
        public string? orgTin { get; set; }
        public string? voucherCode { get; set; }
        public string? date { get; set; }
        public string? adult { get; set; }
        public string? child { get; set; }
        public string? roomTypeCode { get; set; }
        public string? rateCode { get; set; }
        public string? rateCodeDetail { get; set; }
        public string? averageAmount { get; set; }
        public string? totalAmount { get; set; }
        public string? paymentMethod { get; set; }
        public string? roomCount { get; set; }
        public List<GuestModel> guests { get; set; }
        public string? oud { get; set; }
        public string? specialRequirement { get; set; }
        public string? cashRecieptVoucher { get; set; }
        public string? transactionReference { get; set; }
        public OnHotelBookSuccessModel onHotelBookSuccess { get; set; }
        public PaymentInfoModel paymentInfo { get; set; }
    }

    public class GuestModel
    {
        public string? Code { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? IdPicture { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Nationality { get; set; }
    }

    public class OnHotelBookSuccessModel
    {
        public string? firstName { get; set; }
        public string? company { get; set; }
        public string? branch { get; set; }
        public int nightCount { get; set; }
    }

}
