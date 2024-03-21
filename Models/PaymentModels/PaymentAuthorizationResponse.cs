namespace HulubejeBooking.Models.PaymentModels
{
    public class PaymentAuthorizationResponse
    {
        public bool IsSuccessful { get; set; }
        public List<string>? ErrorMessages { get; set; }
        public AdditionalParameter? AdditionalParameters { get; set; }
        public string? TransactionReference { get; set; }
    }

    public class AdditionalParameter
    {
        public string? RedirectUrl { get; set; }
        public string? CustomerName { get; set; }
        public string? Type { get; set; }
        public bool? IsAsyncMode { get; set; }
    }

}
