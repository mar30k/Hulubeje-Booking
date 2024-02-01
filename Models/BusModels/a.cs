namespace HulubejeBooking.Models.BusModels
{
	public class A
	{
		public int RouteId { get; set; }
		public int Distance { get; set; }
		public string OriginTerminalName { get; set; }
		public string ViaName { get; set; }
		public string DestinationCityName { get; set; }
		public string DestinationTerminalName { get; set; }
		public string RouteGroupDesc { get; set; }
	}
	public class RouteModel
	{
		public string OriginCityName { get; set; }
		public List<A> Routes { get; set; }
	}
}
