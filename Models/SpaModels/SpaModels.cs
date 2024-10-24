using HulubejeBooking.Models.CInemaModels;

namespace HulubejeBooking.Models.SpaModels
{
    public class Service
    {
        public int Code { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public List<string>? Pictures { get; set; }
        public string? Wishlist { get; set; }
    }

    public class Child
    {
        public int Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public List<Service>? Services { get; set; }
    }

    public class Categorys
    {
        public int Code { get; set; }
        public string? Category { get; set; }
        public List<Child>? Children { get; set; }
    }
    public class Schedules
    {
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public bool? IsAvailable { get; set; }
    }
    public class CartItem
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }


    public class ScheduleView
    {
        public List<CartItem>? CartItem { get; set; }
        public HulubejeResponse<List<Schedules>>? Schedules { get; set; }
    }
    public class SpaProductsView
    {
        public List<CartItem>? CartItem { get; set; }
        public Child? Child { get; set; }
    }

}
