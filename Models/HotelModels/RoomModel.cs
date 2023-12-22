namespace HulubejeBooking.Models.HotelModels
{
	public class RoomModel
	{
		public string RoomTypeCode { get; set; }
		public string RoomTypeDescription { get; set; }
		public string RoomDescription { get; set; }
		public string RateCode { get; set; }
		public string RatePolicy { get; set; }
		public string RateCodeDescription { get; set; }
		public string RateCodeDetail { get; set; }
		public decimal AverageAmount { get; set; }
		public decimal? OldAverageAmount { get; set; }
		public int? AvailableRoom { get; set; }  // Make AvailableRoom nullable
		public int TotalRoom { get; set; }
		public decimal? Min { get; set; }  // Make Min nullable
		public decimal? Max { get; set; }  // Make Max nullable
		public List<string> PictureUrls { get; set; }
		public List<Amenity> Aminities { get; set; }
		public string PackageList { get; set; }
	}

	public class Amenity
	{
		public int Index { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public string ImageUrl { get; set; }
		public string Description { get; set; }
	}
}
