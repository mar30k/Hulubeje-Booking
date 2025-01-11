using HulubejeBooking.Models.Authentication;
using HulubejeBooking.Models.BusModels;
using HulubejeBooking.Models.HotelModels;

namespace HulubejeBooking.Models.CInemaModels
{
    public class Movie
    {
        public bool? IsSuccessful { get; set; }
        public List<CompanyData>? Data { get; set; }
        public List<CompanyData>? TrendingMovies { get; set; }
        public string[]? ErrorMessages { get; set; }
        public string[]? AdditionalParameters { get; set; }
        public GetcompaniesbyType? Companies { get; set; }

    }

    public class CompanyData
    {
        public int? CompanyCode { get; set; }
        public string? CompanyName { get; set; }
        public string? TIN { get; set; }
        public int? BranchCode { get; set; }
        public string? BranchName { get; set; }
        public List<Movies>? Movies { get; set; }
        public bool IsTrendingDown { get; set; }
        public bool IsTrendingUp { get; set; }
        public Stat? Stat { get; set; }
        public double Rating { get; set; }
        public int RatingCount { get; set; }
        public List<Companies>? Companies { get; set; }
    }
    public class Stat
    {
        public Count? Count { get; set; }
        public Place? Place { get; set; }
    }

    public class Count
    {
        public int Day { get; set; }
        public int Week { get; set; }
        public int Month { get; set; }
    }

    public class Place
    {
        public string Day { get; set; }
        public string Week { get; set; }
        public string Month { get; set; }
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
        public decimal Rating { get; set; }
        public int RatingCount { get; set; }
        public string? RatingName { get; set; }
        public string? Duration { get; set; }
        public string? Director { get; set; }
        public string? Writer { get; set; }
        public string? Actors { get; set; }
        public string? StreamUrl { get; set; }
        public string? MoviePoster { get; set; }
        public string? SupplierConsigneeId { get; set; }
        public string? SupplierConsigneeUnit { get; set; }
        public string? SupplierConsigneeName { get; set; }
        public string? SupplierConsigneeUnitName { get; set; }
        public List<MovieSchedules>? MovieSchedule { get; set; }
    }

    public class MovieSchedules
    {
        public string? ParentCinemaHall { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? DimensionName { get; set; }
        public string? Status { get; set; }
        public int SchdetailId { get; set; }
        public List<MovieSpace>? MovieSpaces { get; set; }
    }

    public class MovieSpace
    {
        public int? Id { get; set; }
        public int? SpaceId { get; set; }
        public string? CinemaHall { get; set; }
        public decimal? PriceValue { get; set; }
        public int? DefaultTax { get; set; }
    }
    public class Companies
    {
        public string? Tin { get; set; }
        public string? BranchCode { get; set; }
        public string? CompanyName { get; set; }
    }
}
