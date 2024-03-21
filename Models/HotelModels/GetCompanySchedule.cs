namespace HulubejeBooking.Models.HotelModels
{
    public class Amenities
    {
        public int? Index { get; set; }
        public int? Code { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
    }

    public class CompanySchedule
    {
        public string? Status { get; set; }
        public List<object>? Schedules { get; set; }
        public List<Amenity>? Aminities { get; set; }
    }

    public class GetCompanySchedule
    {
        public bool? IsSuccessful { get; set; }
        public CompanySchedule? Data { get; set; }
        public List<string>? ErrorMessages { get; set; }
        public List<string>? AdditionalParameters { get; set; }
    }
}
