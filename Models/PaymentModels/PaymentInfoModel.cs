namespace HulubejeBooking.Models.PaymentModels
{
    public class PaymentInfoModel
    {
        public string UserAccessToken { get; set; }
        public PaymentTransactionRequest PaymentTransactionRequest { get; set; }
        public bool IsAsyncMode { get; set; }
    }

    public class PaymentTransactionRequests
    {
        public string UserMobileNumber { get; set; }
        public string SupplierTin { get; set; }
        public string SupplierOUD { get; set; }
        public string TransactionId { get; set; }
        public double Amount { get; set; }
        public string PaymentProviderOUD { get; set; }
        public string Pin { get; set; }
        public string ExpirationDate { get; set; }
    }

}
