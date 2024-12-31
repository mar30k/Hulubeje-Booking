using HulubejeBooking.Models.PaymentModels;

namespace HulubejeBooking.Models.HotelModels
{
    public class GetCompanyImages
    {
        public bool IsSuccessful { get; set; }
        public List<string>? Data { get; set; }
        public List<string>? ErrorMessages { get; set; }
        public List<string>? AdditionalParameters { get; set; }
    }

    public class CompanyDetailModel
    {
        public GetCompanySchedule? CompanySchedule { get; set; }
        public PaymentProcessorResponse? PaymentOptions { get; set; }
        public GetCompanyImages? ImageModel { get; set; }
        public string? Description { get; set; }
        public string? Name { get; set; }
        public int? CompanyCode { get; set; }
        public int? OrgOUD { get; set; }
        public string? BranchName { get; set; }
        public string? CityName { get; set; }
        public string? Department {  get; set; } 
        public int? Duration {  get; set; } 
        public int? ArticleCode {  get; set; } 
        public int? Code {  get; set; } 
        public int? Organizer {  get; set; } 
        public DateTime? SelectedDate { get; set; }
    }
}
