namespace HulubejeBooking.Models.HotelModels
{
    public class RoomType
    {
        public int? RoomTypeCode { get; set; }
        public string? RoomTypeDescription { get; set; }
        public string? RoomDescription { get; set; }
        public int? RateCode { get; set; }
        public string? RatePolicy { get; set; }
        public string? RateCodeDescription { get; set; }
        public int? RateCodeDetail { get; set; }
        public double? AverageAmount { get; set; }
        public double? TotalAmount { get; set; }
        public double? OldAverageAmount { get; set; }
        public int? AvailableRoom { get; set; }
        public string? VideoUrl { get; set; }
        public int? TotalRoom { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }
        public List<string>? PictureUrls { get; set; }
        public List<Aminity>? Aminities { get; set; }
        public List<Aminity>? HotelAminities { get; set; }
        public string? Packagelist { get; set; }
    }

    public class Aminity
    {
        public int? Index { get; set; }
        public int? Code { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
    }

    public class GetRooms
    {
        
        public bool? IsSuccessful { get; set; }
        public string? HotelName { get; set; }
        public List<RoomType>? Data { get; set; }
        public List<string>? ErrorMessages { get; set; }
        public List<string>? AdditionalParameters { get; set; }
    }
}
