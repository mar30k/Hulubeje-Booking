namespace HulubejeBooking.Models.CInemaModels
{
    public class FoodItem
    {
        public bool? IsSuccessful { get; set; }
        public List<FoodCategory>? Data { get; set; }
        public string[]? ErrorMessages { get; set; }
        public object? AdditionalParameters { get; set; }
    }

    public class FoodCategory
    {
        public int? Code { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public List<FoodCategory>? Children { get; set; }
        public decimal? Price { get; set; }
        public string? Currency { get; set; }
        public string? Description { get; set; }
        public List<string>? Pictures { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
        public string? Brand { get; set; }
        public string? Weight { get; set; }
        public string? Model { get; set; }
        public string? Manufacturer { get; set; }
        public string? CountryOrigin { get; set; }
        public decimal? PreviousValue { get; set; }
        public string? PackageForm { get; set; }
    }


}
