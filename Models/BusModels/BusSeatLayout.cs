namespace HulubejeBooking.Models.BusModels
{
    public class BusSeatLayout
    {
        public string? PlateNumber { get; set; }
        public string? Terminal { get; set; }
        public string? Distance { get; set; }
        public string? Tariff { get; set; }
        public string? Level { get; set; }
        public string? Route { get; set; }
        public string? OperatorName { get; set; }
        public string? Time {  get; set; }  
        public string? Date {  get; set; }  
        public string? DestinationCity {  get; set; }  
        public string? DepatureCity {  get; set; }
        public string? ArrivialDate {  get; set; }
        public string? DepartureDate {  get; set; }
        public string? VehicleOperatorId {  get; set; }
        public SeatLayout? SeatLayout { get; set; }

    }
    public class SeatLayout
    {
        public int? Id { get; set; }
        public int? MaxX { get; set; }
        public int? MaxY { get; set; }
        public List<Seat>? Seats { get; set; }
    }

    public class Seat
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public int? X { get; set; }
        public int? Y { get; set; }
        public string? Type { get; set; }
    }
}
