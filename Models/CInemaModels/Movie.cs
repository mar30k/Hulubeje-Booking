namespace HulubejeBooking.Models.CInemaModels
{
    public class Movie
    {
        public bool? IsSuccessful { get; set; }
        public List<CompanyData>? Data { get; set; }
        public string[]? ErrorMessages { get; set; }
        public object? AdditionalParameters { get; set; }
    }

    public class CompanyData
    {
        public int? CompanyCode { get; set; }
        public string? CompanyName { get; set; }
        public string? TIN { get; set; }
        public int? BranchCode { get; set; }
        public string? BranchName { get; set; }
        public List<Movies>? Movies { get; set; }
    }

    public class Movies
    {
        public DateTime? Date { get; set; }
        public int? MovieId { get; set; }
        public string? MovieName { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? DayMonth { get; set; }
        public int? ConsigneeUnit { get; set; }
        public int? Article { get; set; }
        public string? Plot { get; set; }
        public string? MovieLanguage { get; set; }
        public string? MovieLanguageDescription { get; set; }
        public string? SubtitleLanguage { get; set; }
        public string? MovieCategoryName { get; set; }
        public string? RatingName { get; set; }
        public string? Duration { get; set; }
        public string? Director { get; set; }
        public string? Writer { get; set; }
        public string? Actors { get; set; }
        public string? StreamUrl { get; set; }
        public string? MoviePoster { get; set; }
        public List<MovieSchedules>? MovieSchedule { get; set; }
    }

    public class MovieSchedules
    {
        public string? ParentCinemaHall { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? DimensionName { get; set; }
        public string? Status { get; set; }
        public int? SchdetailId { get; set; }
        public List<MovieSpace>? MovieSpaces { get; set; }
    }

    public class MovieSpace
    {
        public int?  Id { get; set; }
        public int? SpaceId { get; set; }
        public string? CinemaHall { get; set; }
        public decimal? PriceValue { get; set; }
        public int? DefaultTax { get; set; }
    }

}
