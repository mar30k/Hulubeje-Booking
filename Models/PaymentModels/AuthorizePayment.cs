using HulubejeBooking.Models.PaymentModels.HotlePaymentModels;

namespace HulubejeBooking.Models.PaymentModels
{
    public class AuthorizePayment
    {
       
        public string? UserMobileNumber { get; set; }
        public string? SupplierTin { get; set; } 
        public string? SupplierOUD { get; set; } 
        public string? TransactionId { get; set; } 
        public double? Amount { get; set; } = 0.1;
        public string? PaymentProviderOUD { get; set; }
        public AdditionalParametersModel AdditionalParameters { get; set; }

    }
    public class AdditionalParametersModel
    {
        public string? ReferenceNumber { get; set; } = "Optional";
    }
    
}
