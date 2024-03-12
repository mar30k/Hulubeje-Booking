namespace HulubejeBooking.Models.CInemaModels
{
    public class ProductsViewModel
    {
        public List<Product>? Products { get; set; }
        public FoodItem? FoodItem { get; set; }
        public string? MovieScheduleCode { get; set; }
        public string? ArticleCode { get; set; }
        public string? CompanyTinNumber { get; set; }
        public string? CompanyName { get; set; }
        public string? SpaceType { get; set; }
        public string? MovieName { get; set; }
        public string? ScheduleDate { get; set; }
        public string? ScheduleTime { get; set; }
        public string? HallName { get; set; }
        public string? Dimension { get; set; }
        public string? SeatCacheKey { get; set; }
        public string? NumberOfSeats { get; set; }
        public string? BranchCode { get; set; }
        public decimal Price { get; set; }
        public Bill? Bill { get; set; }
        public List<SelectedItem>? SelectedItems { get; set; }

    }

}
