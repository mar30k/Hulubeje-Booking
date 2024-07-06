using HulubejeBooking.Models.BusModels;
using HulubejeBooking.Models.CInemaModels;
using Newtonsoft.Json;

namespace HulubejeBooking.Models
{
    public class HistoryWrapper
    {
        public OrdersModel? OrdersModel { get; set; }
        public List<HistoryModel>? HistoryModel { get; set; }
    }
    //public class Orders
    //{
    //    public string? VoucherCode { get; set; }
    //    public string? VoucherType { get; set; }
    //    public string? ConsigneeCode { get; set; }
    //    public DateTime? IssuedDate { get; set; }
    //    public decimal? GrandTotal { get; set; }
    //    public string? ObjectState { get; set; }
    //    public string? SupplierBrandName { get; set; }
    //    public string? OrgTin { get; set; }
    //    public string? OrgPreviewImage { get; set; }
    //    public string? Industry { get; set; }
    //    public string? BranchCode { get; set; }
    //    public string? BranchName { get; set; }
    //    public double? Latitude { get; set; }
    //    public double? Longitude { get; set; }
    //}

    public class OrdersModel
    {
        public bool? IsSuccessful { get; set; }
        public List<VoucherData>? Data { get; set; }
        public List<string>? ErrorMessages { get; set; }
        public List<string>? AdditionalParameters { get; set; }
    }
    public class RatingResponse
    {
        public bool? IsSuccessful { get; set; }
        public bool? Data { get; set; }
        public List<string>? ErrorMessages { get; set; }
        public List<string>? AdditionalParameters { get; set; }
    }
    public class VoucherData
    {
        public int? CompanyCode { get; set; }
        public string? CompanyName { get; set; }
        public int? BranchCode { get; set; }
        public string? BranchName { get; set; }
        public int? IndustryType { get; set; }
        public string? VoucherCode { get; set; }
        public DateTime IssuedDate { get; set; }
        public decimal? GrandTotal { get; set; }
        public string? Logo { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public object? Articles { get; set; }
    }

    public class Ratings
    {
        public int? BranchCode { get; set; }
        public int? Rating { get; set; }
        public string? Review { get; set; }
        public string? Code { get; set; }
        public object? Article { get; set; }
    }

    public class GetHistoryDetailResposne
    {
        public bool? IsSuccessful { get; set; }
        public Data? Data { get; set; }
        public List<string>? ErrorMessages { get; set; }
        public List<string>? AdditionalParameters { get; set; }
        public string? QRCodeImage { get; set; }
    }

    public class Data
    {
        public List<LineItem>? LineItems { get; set; }
        public ExtraCharge? ExtraCharge { get; set; }
        public decimal? GrandTotal { get; set; }
        public Dictionary<string, object>? ExtraInformation { get; set; }
        public ExtraData? ExtraData { get; set; }
    }

    public class ExtraData
    {
        public int? VoucherId { get; set; }
        public string? Tin { get; set; }
        [JsonProperty("status:")]
        public string? Status { get; set; }

    }
}
