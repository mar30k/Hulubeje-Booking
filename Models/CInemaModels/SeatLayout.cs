namespace HulubejeBooking.Models.CInemaModels
{
    public class SeatLayout
    {
        public string? SpaceCode { get; set; }
        public string? CompanyTinNumber { get; set; }
        public string? CompanyName { get; set; }
        public string? MovieName { get; set; }
        public string? ArticleCode { get; set; }
        public string? SelectedDate { get; set; }
        public string? HallName { get; set; }
        public string? UtcTime { get; set; }
        public string? SpaceType { get; set; }
        public string? Dimension { get; set; }
        public string? MovieScheduleCode { get; set; }
        public int MaxX { get; set; }
        public int MaxY { get; set; }
        public decimal Price { get; set; }
        public List<SeatInfo>? Seats { get; set; }
        public string? MaxSeats { get; set; }
        public List<string>? BookedSeats { get; set; }
        public List<string>? SoldSeats { get; set; }
        public List<string>? AvailableSeats { get; set; }
        public List<string>? TakenSeats { get; set; }

    }

    public class SeatInfo
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string? Type { get; set; }
    }


}
