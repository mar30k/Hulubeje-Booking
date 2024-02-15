namespace HulubejeBooking.Models.BusModels
{
    public class HistoryModel
    {
        public int? Id { get; set; }
        public string? Payer { get; set; }
        public int? RouteSchedule { get; set; }
        public DateTime IssuedDate { get; set; }
        public string? RouteDescription { get; set; }
        public DateTime DepartureDate { get; set; }
        public int? Distance { get; set; }
        public string? TravelDuration { get; set; }
        public string? DriverName { get; set; }
        public string? AssistantName { get; set; }
        public DateTime ArrivalDate { get; set; }
        public string? Gate { get; set; }
        public string? RouteName { get; set; }
        public string? LevelDesc { get; set; }
        public int? SideNumber { get; set; }
        public string? VehicleOperator { get; set; }
        public string? ImgUrl { get; set; }
        public string? Plate { get; set; }
        public string? ViaDescription { get; set; }
        public string? OriginTerminalName { get; set; }
        public string? DestinationTerminalName { get; set; }
        public string? DestCityName { get; set; }
        public string? PassengerIdNumber { get; set; }
        public string? PassengerNationalId { get; set; }
        public string? PassengerFullName { get; set; }
        public string? PickupLocationName { get; set; }
        public string? SeatLayoutName { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Discount { get; set; }
        public decimal? AdditionalCharge { get; set; }
        public decimal? GrandTotal { get; set; }
    }
}
