namespace HulubejeBooking.Models.HotelModels
{
    public class CityDatas
    {
        public int? Id { get; set; }
        public int? Hotels { get; set; }
        public int? Country { get; set; }
        public string? CountryName { get; set; }
        public string? CityName { get; set; }
        public string? CityImage { get; set; }
        public string? AlternativeName { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? Population { get; set; }
    }

    public class GetCities
    {
        public bool IsSuccessful { get; set; }
        public List<CityDatas>? Data { get; set; }
        public List<string>? ErrorMessages { get; set; }
        public List<string>? AdditionalParameters { get; set; }
    }
}
