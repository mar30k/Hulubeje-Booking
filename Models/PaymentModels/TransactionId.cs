namespace HulubejeBooking.Models.PaymentModels
{
    public class TransactionId
    {
        public bool IsSuccessful { get; set; }
        public string? Data { get; set; }
        public List<string>? ErrorMessages { get; set; }
        public List<string>? AdditionalParameters { get; set; }
    }

}
