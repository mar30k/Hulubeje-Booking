using HulubejeBooking.Models.BusModels;
using HulubejeBooking.Models.CInemaModels;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace HulubejeBooking.Models
{
    public class HistoryWrapper
    {
        public HulubejeResponse<List<VoucherData>>? OrdersModel { get; set; }
        public HulubejeResponse<List<VoucherData>>? PayementsHistory { get; set; }
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
        public string? Tin { get; set; }
        public string? AttachmentLink { get; set; }
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
        public GetHistoryResponse? Data { get; set; }
        public List<string>? ErrorMessages { get; set; }
        public List<string>? AdditionalParameters { get; set; }
        public string? QRCodeImage { get; set; }
        public VoucherData? VoucherData { get; set; }

    }

    public class GetHistoryResponse
    {
        public List<LineItem>? LineItems { get; set; }
        public ExtraCharge? ExtraCharge { get; set; }
        public decimal? GrandTotal { get; set; }
        public Dictionary<string, object>? ExtraInformation { get; set; }
        public ExtraData? ExtraData { get; set; }
        public DateTime? IssuedDate { get; set; }
        public int? BranchCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CompanyName { get; set; }
        public string? VoucherCode { get; set; }
        public string? PromoDetail { get; set; }

    }

    public class ExtraData
    {
        public int? VoucherId { get; set; }
        public string? Tin { get; set; }
        [JsonProperty("status:")]
        public string? Status { get; set; }

    }
    public class ReviewResponse
    {
        public double Rating { get; set; }
        public int Count { get; set; }
        public int BranchCode { get; set; }
        public List<Reviews>? Reviews { get; set; }
    }
    public class GetReviewsRequest
    {
        public int BranchCode { get; set; }
        public bool RetriveAllReviews { get; set; } = true;
    }
    public class Reviews
    {
        public string? Image { get; set; }
        public string? FullName { get; set; }
        public string? ReviewerPhoneNumber { get; set; }
        public bool IsVerifiedUser { get; set; }
        [JsonPropertyName("review")]
        public string? Review { get; set; }
        public string? VoucherCode { get; set; }
        public string? Reply { get; set; }
        public string? ReferenceVoucher { get; set; }
        public string? Attachment { get; set; }
        public double Rating { get; set; }
        public DateTime Date { get; set; }
    }

    public class HulubejeResponse<T>
    {
        public bool? IsSuccessful { get; set; }
        public T? Data { get; set; }
        public List<string>? ErrorMessages { get; set; }
        public Dictionary<string, string>? AdditionalParameters { get; set; }
    }
}
