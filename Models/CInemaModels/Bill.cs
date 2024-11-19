using Newtonsoft.Json;

namespace HulubejeBooking.Models.CInemaModels
{
    public class LineItem
    {
        public int? Article { get; set; }
        public string? Name { get; set; }
        public decimal? UnitAmount { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? TaxableAmount { get; set; }
    }

    public class ExtraCharge
    {
        [JsonProperty("TXBL 1")]
        public decimal? TXBL1 { get; set; }
        [JsonProperty("TAX1 15%")]
        public decimal? TAX115 { get; set; }
        public decimal? AddCharge { get; set; }
    }


    public class Bill
    {
        public bool? IsSuccessful { get; set; }
        public Data? Data { get; set; }
        public List<string>? ErrorMessages { get; set; }
        public Dictionary<string, object>? AdditionalParameters { get; set; }
    }
}
