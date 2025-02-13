namespace HulubejeBooking.Models.CInemaModels
{
    public class SeatLayouts
    {
        public bool? IsSuccessful { get; set; }
        public SeatLayout? Data { get; set; }
        public string[]? ErrorMessages { get; set; }
        public object? AdditionalParameters { get; set; }
        public List<SeatStatus>? SeatStatus { get; set; }
    }
    public class SeatLayout
    {
        public string? SpaceCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? MovieCode { get; set; }
        public string? CompanyTinNumber { get; set; }
        public string? CompanyCode { get; set; }
        public string? CompanyName { get; set; }
        public string? MovieName { get; set; }
        public string? ArticleCode { get; set; }
        public string? SelectedDate { get; set; }
        public string? HallName { get; set; }
        public string? UtcTime { get; set; }
        public string? SpaceType { get; set; }
        public string? Dimension { get; set; }
        public string? MovieScheduleCode { get; set; }
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }
        public string? CinemaHall { get; set; }
        public int MaxX { get; set; }
        public int MaxY { get; set; }
        public int SpaceId { get; set; }
        public string? Id { get; set; }
        public decimal? Price { get; set; }
        public decimal? OldPriceValue { get; set; }
        public List<SeatInfo>? Seats { get; set; }
        public string? MaxSeats { get; set; }

    }

    public class SeatInfo
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string? Type { get; set; }
    }
    public class SeatStatus
    {
        public string? Status { get; set; }
        public int Value { get; set; }
    }
    public class EntryExtension
    {
        public string? Key { get; set; }
        public int? Extension { get; set; }
        public string? ExtensionDeligate { get; set; }
    }
    public class SafePushEntry
    {
        public string? Key { get; set; }
        public int? Value { get; set; }
    }
    public class SafePushEntryResponse
    {
        public string? Status { get; set; }
        public bool? IsSuccessful { get; set; }
    }
}
