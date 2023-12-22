namespace HulubejeBooking.Models.CInemaModels
{
    public class SelectedItem
    {
        public string? articleName { get; set; }
        public int quantity { get; set; }
        public string? articleCode { get; set; }
        public decimal price { get; set; }
        public string? lineItemNote { get; set; }
        public string? specialFlag { get; set; }
    }
}
