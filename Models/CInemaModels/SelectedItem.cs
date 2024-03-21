namespace HulubejeBooking.Models.CInemaModels
{
    public class SelectedItem
    {
        public string? name { get; set; }
        public int quantity { get; set; }
        public decimal unitAmount { get; set; }
        public int? article { get; set; }
        public int? uom { get; set; }
        public string? specialFlag { get; set; }
    }
}
