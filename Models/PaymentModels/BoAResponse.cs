namespace HulubejeBooking.Models.PaymentModels
{
    public class BoAResponse
    {
        public string transactionReference { get; set; }
        public bool isSuccessful { get; set; }
        public List<string> errorMessages { get; set; }
        public BoAAdditionalParametersModel additionalParameters { get; set; }
    }

    public class BoAAdditionalParametersModel
    {
        public string RedirectUrl { get; set; }
    }
    public class PaymentResponse
    {
        public string cardText { get; set; }
        public string RedirectUrl { get; set; }
    }
}
