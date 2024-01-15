namespace HulubejeBooking.Models.HotelModels
{
    public class GetModel
    {

        public string Code { get; set; }
        public string TradeName { get; set; }
        public string? BrandName { get; set; }
        public string IndustryType { get; set; }
        public bool IsTaxInclusive { get; set; }
        public string RegisterDate { get; set; }
        public double Rating { get; set; }
        public double RatingCount { get; set; }
        public bool IsSponsored { get; set; }
        public string? TIN { get; set; }
        public List<string> TermsAndConditions { get; set; }
        public List<string> Attachments { get; set; }
        public List<BranchModel> Branches { get; set; } = new List<BranchModel>();
        public string? TermsAndConditionsUrl { get; set; }
        public string? OUD { get; set; }
        public string? BranchName { get; set; }
        public string? BranchCategory { get; set; }
    }

    public class BranchModel
    {
        public string Code { get; set; }
        public string BranchName { get; set; }
        public string? City { get; set; }
        public string Note { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double BranchRating { get; set; }
        public string Logo { get; set; }
        public string Category { get; set; }
    }

    public class CityData
    {
        public string? CityName { get; set; }
        public int BranchesCount { get; set; }
        public string? CityImageUrl { get; set; }
    }

    public class HotelListRequest
    {
        public List<GetModel> HotelList { get; set; }
        public List<CityData> cityName { get; set; }
        public List<CityData>? CityNameList { get; internal set; }
    }
}
