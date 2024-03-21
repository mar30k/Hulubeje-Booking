using HulubejeBooking.Models.PaymentModels;

namespace HulubejeBooking.Models.HotelModels
{
	public class PaymentOption
    {
        public string? PaymentMethodName { get; set; }
        public string? PaymentMethodOud { get; set; }
        public string? PaymentMethodImage { get; set; }
    }
    public class GetCompanyImages
    {
        public bool IsSuccessful { get; set; }
        public List<string>? Data { get; set; }
        public List<string>? ErrorMessages { get; set; }
        public List<string>? AdditionalParameters { get; set; }
    }

    public class HotelDetailModel
    {
        public GetCompanySchedule? CompanySchedule { get; set; }
        public PaymentProcessorResponse? PaymentOptions { get; set; }
        public GetCompanyImages? ImageModel { get; set; }
        public string? Description { get; set; }
        public string? Name { get; set; }
        public int? CompanyCode { get; set; }
        public int? OrgOUD { get; set; }

    }
}
