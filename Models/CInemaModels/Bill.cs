namespace HulubejeBooking.Models.CInemaModels
{
    public class Bill
    {
        public decimal TakeAwayBoxCharge { get; set; }
        public Extras Extras { get; set; }
        public List<TaxInformation> TaxInformation { get; set; }
        public decimal TotalAmount { get; set; }
        public List<LineItem> LineItems { get; set; }
    }

    public class TaxInformation
    {
        public decimal TaxAmount { get; set; }
        public string? TaxInPercentage { get; set; }
        public int TaxType { get; set; }
    }

    public class LineItem
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TaxableAmount { get; set; }
        public int TaxType { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string LineItemNote { get; set; }
        public string SpecialFlag { get; set; }
    }

    public class Extras
    {
        public decimal AdditionalCharge { get; set; }
        public decimal Discount { get; set; }
        public decimal SubTotal { get; set; }
    }
}
