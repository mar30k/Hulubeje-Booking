namespace HulubejeBooking.Models.BusModels
{
    public class Routes
    {
        public List<City>? Cities { get; set; }
        public List<Route>? Route { get; set; }
    }
    public class City
    {
        public string? CityName { get; set; }
        public int? CityCode { get; set; }
    }

    public class Route 
    {
        public int? FromCity { get; set; }
        public List<ToCity>? ToCity { get; set; }
    }

    public class ToCity
    {
        public int? RouteId { get; set; }
        public int? CityCode { get; set; }
    }
}
