namespace HulubejeBooking.Models.PaymentModels
{
    public class PaymentValidation
    {
        public bool? isSuccessful { get; set; }
        public List<string>? errorMessages { get; set; }
        public AdditionalParameters? additionalParameters { get; set; }
        public string? transactionReference { get; set; }
 
    }
    public class AdditionalParameters 
    {
        public string? CustomerName { get; set; }
    }
}
