namespace HulubejeBooking.Models.HotelModels
{
    public class HotelViewModel
    {
        public List<HotelModel> Hotels { get; set; }
        public List<RoomModel> Rooms { get; set; }
        public List<Confirmation>? Confirm { get; internal set; }
    }
}
