namespace HulubejeBooking.Models.HotelModels
{
    public class Hotel
    {
        public int? OrganizationCode { get; set; }
        public string? OrganizationName { get; set; }
        public string? OrgTin { get; set; }
        public string? BranchName { get; set; }
        public int? BranchCode { get; set; }
        public string? BranchCategory { get; set; }
        public string? CityName { get; set; }
        public string? RateDescription { get; set; }
        public string? PortraitPicture { get; set; }
        public string? SpecificLocation { get; set; }
        public DateTime? RegisterDate { get; set; }
        public List<string>? Attachments { get; set; }
        public double? MinRoomValue { get; set; }
        public double? HotelRating { get; set; }
        public int? HotelRoomsCount { get; set; }
        public double? TotalPrice { get; set; }
        public bool? IsTaxInclusive { get; set; }
        public List<string>? RoomAminities { get; set; }
        public List<string>? HotelAminities { get; set; }
    }

    public class GetHotelByCity
    {
        public bool? IsSuccessful { get; set; }
        public List<Hotel>? Data { get; set; }
        public List<string>? ErrorMessages { get; set; }
        public List<string>? AdditionalParameters { get; set; }
        public RoomFormData? RoomFormData { get; set; }
    }
}
