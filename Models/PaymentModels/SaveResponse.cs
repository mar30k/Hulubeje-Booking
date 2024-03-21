namespace HulubejeBooking.Models.PaymentModels
{
    public class SaveResponse
    {
        public bool? IsSuccessful { get; set; }
        public List<string>? ErrorMessages { get; set; }
        public List<string>? AdditionalParameters { get; set; }
        public string? TransactionReference { get; set; }
    }
}
