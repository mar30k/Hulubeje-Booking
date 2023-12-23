namespace HulubejeBooking.Models.HotelModels
{
    public class AvailabilityViewModel
    {
        public List<RoomModel> AvailableRooms { get; set; }
		public List<string> AllPictureUrls
		{
			get => AvailableRooms?.SelectMany(room => room.PictureUrls).ToList() ?? new List<string>();
			set { }
		}
	}
}
