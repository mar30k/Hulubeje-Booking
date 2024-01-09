namespace HulubejeBooking.Models.PaymentModels
{
    public class PaymentResponseModel
    {
        public bool IsResolved { get; set; }
        public bool IsFulfilled { get; set; }
        public string Remark { get; set; }
        public string PaymentProviderTransactionId { get; set; }
        public string PaymentProviderOUD { get; set; }
        public string PaymentProviderOrganizationTin { get; set; }
        public string PaymentProviderName { get; set; }
        public string TransactionTime { get; set; }
        public decimal Amount { get; set; }
        public AdditionalParameterModel AdditionalParameters { get; set; }
    }

    public class AdditionalParameterModel
    {
        public string CustomerName { get; set; }
        public string CustomerMobileNumber { get; set; }
    }
}
