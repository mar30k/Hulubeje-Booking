namespace HulubejeBooking.Models.CInemaModels
{
    public class Product
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? ProductType { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public decimal? Price { get; set; }
        public int? TaxType { get; set; }
        public decimal? TaxValue { get; set; }
        public List<string>? Pictures { get; set; }
        public List<Product>? Children { get; set; }
        public bool? IsInWishList { get; set; }
        public int? StockInformation { get; set; }
        public bool? HasPrescription { get; set; }
        public double? Rating { get; set; }
    }

}
