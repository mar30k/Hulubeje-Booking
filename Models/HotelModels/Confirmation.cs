namespace HulubejeBooking.Models.HotelModels
{
    public class Confirmation
    {
        public bool IsSuccessful { get; set; }
        public List<string> ErrorMessages { get; set; }
        public object AdditionalParameters { get; set; }  
        public string TransactionReference { get; set; }
    }
}
