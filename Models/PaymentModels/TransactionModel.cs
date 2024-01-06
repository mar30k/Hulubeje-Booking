namespace HulubejeBooking.Models.PaymentModels
{
    public class TransactionModel
    {
        public string UserMobileNumber { get; set; }
        public string SupplierTin { get; set; } 
        public string SupplierOUD { get; set; } 
        public string TransactionId { get; set; }
        public double Amount { get; set; } = 1.0;
        public string PaymentProviderOUD { get; set; }
        public string? Pin { get; set; } 

        public AdditionalParametersModel AdditionalParameters { get; set; }
    }

    
}

