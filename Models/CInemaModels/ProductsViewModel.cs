namespace HulubejeBooking.Models.CInemaModels
{
    public class ProductsViewModel
    {
        public List<Product>? Products { get; set; }
        public FoodItem? FoodItem { get; set; }
        public int? MovieScheduleCode { get; set; }
        public int? ArticleCode { get; set; }
        public string? CompanyTinNumber { get; set; }
        public string? CompanyName { get; set; }
        public int? CompanyCode { get; set; }
        public string? SpaceType { get; set; }
        public string? MovieName { get; set; }
        public DateTime? ScheduleDate { get; set; }
        public string? ScheduleTime { get; set; }
        public string? HallName { get; set; }
        public string? Dimension { get; set; }
        public string? SeatCacheKey { get; set; }
        public string? Seats { get; set; }
        public int? BranchCode { get; set; }
        public int? SpaceID { get; set; }
        public string? BranchName { get; set; }
        public decimal Price { get; set; }
        public Bill? Bill { get; set; }
        public List<SelectedItem>? SelectedItems { get; set; }

    }

}
