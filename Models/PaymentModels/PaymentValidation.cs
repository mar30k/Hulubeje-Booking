namespace HulubejeBooking.Models.PaymentModels
{
    public class PaymentValidation
    {
        public bool? isSuccessful { get; set; }
        public List<string>? errorMessages { get; set; }
        public string? additionalParameters { get; set; }
        public string? transactionReference { get; set; }
    }
}
