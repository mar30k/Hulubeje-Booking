namespace HulubejeBooking.Models
{
    public class Orders
    {
        public string? VoucherCode { get; set; }
        public string? VoucherType { get; set; }
        public string? ConsigneeCode { get; set; }
        public DateTime? IssuedDate { get; set; }
        public decimal? GrandTotal { get; set; }
        public string? ObjectState { get; set; }
        public string? SupplierBrandName { get; set; }
        public string? OrgTin { get; set; }
        public string? OrgPreviewImage { get; set; }
        public string? Industry { get; set; }
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class OrdersModel
    {
        public int? NextPage { get; set; }
        public List<Orders>? Orders { get; set; }
    }
}
