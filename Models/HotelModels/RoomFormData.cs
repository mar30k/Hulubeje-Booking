using HulubejeBooking.Models.Authentication;

namespace HulubejeBooking.Models.HotelModels
{
    public class RoomFormData
    {
        public string? Date { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public DateTime? DepartureDate { get; set; }
        public int? orgCode { get; set; }
        public int? numberOfNights { get; set; }
        public int? city { get; set; }
        public int? oud { get; set; }
        public int adultCount { get; set; }
        public int roomsCount { get; set; }
        public int childrenCount { get; set; }
        public string? Name { get; set; }
        public string? BranchName { get; set; }
        public string? orgTin { get; set; }
        public string? Description { get; set; }
        public string? CityName { get; set; }
        public cookieValidation? CookieValidation { get;  set; }
    }
}
