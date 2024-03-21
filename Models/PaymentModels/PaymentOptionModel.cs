namespace HulubejeBooking.Models.PaymentModels
{
    public class PaymentProcessorResponse
    {
        public bool? IsSuccessful { get; set; }
        public List<PaymentProcessorData>? Data { get; set; }
        public List<string>? ErrorMessages { get; set; }
        public List<string>? AdditionalParameters { get; set; }
    }

    public class PaymentProcessorData
    {
        public string? Code { get; set; }
        public string? AccountNo { get; set; }
        public int? OperationMode { get; set; }
        public decimal? BalanceAmount { get; set; }
        public string? PaymentProcessorName { get; set; }
        public string? PaymentProcessorUnitName { get; set; }
        public int? PaymentProcessorConsigneeId { get; set; }
        public int? PaymentProcessorConsigneeUnit { get; set; }
        public int? PaymentProcessorSpecialization { get; set; }
        public string? PaymentProcessorImage { get; set; }
        public string? PaymentProcessorUnitImage { get; set; }
    }
}
