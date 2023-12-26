using HulubejeBooking.Models.BusModels;
namespace HulubejeBooking.Models.BusModels
{
    public class BusModel
    {
        public List<CompanyModel>? Company { get; set; }
        public RouteConfiguration? RouteConfiguration { get; set; }
    }
}
