namespace HulubejeBooking.Models.HotelModels

{
    public class HotelModel
    {
        public string OrganizationName { get; set; }
        public string orgTin { get; set; }
        public double TotalPrice { get; set; }  
        public string RateDescription { get; set; } 
        public string PortraitPicture { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public string HotelRating { get; set; }
        public string SpecificLocation { get; set; }
        public double MinRoomValue { get; set;} 
        public string OnlineRating { get; set; }
        public Boolean IsTaxInclusive { get; set; }

        public int HotelRoomsCount { get; set; }
        public List<string> roomAminities { get; set; }
        public List<string> hotelAminities { get; set; }
    }
}
