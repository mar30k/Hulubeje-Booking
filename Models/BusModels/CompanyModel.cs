namespace HulubejeBooking.Models.BusModels
{
    public class CompanyModel
    {
        public int Id { get; set; }
        public string?  Code { get; set; }
        public string? TradeName { get; set; }
        public string? CompanyName { get; set; }
        public string? Tin { get; set; }
        public DateTime? EstablishmentDate { get; set; }
        public int NumberOfShareholders { get; set; }
        public int SideNumStart { get; set; }
        public int SideNumEnds { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? Woreda { get; set; }
        public string? HouseNum { get; set; }
        public string? SpecificAddress { get; set; }
        public string? CountryName { get; set; }
        public string? CountryPoliticalName { get; set; }
        public string? CityName { get; set; }   
        public string? RegionName { get; set; }
        public string? SubCityName { get; set; }
        public string? CompanyTypeDesc { get; set; }
        public string? ServiceLevelDesc { get; set; }
    }
}
