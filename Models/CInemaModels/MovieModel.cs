namespace HulubejeBooking.Models.CInemaModels
{
    public class MovieModel
    {
        public string? MovieCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? BackdropPath { get; set; }
        public string? ArticleCode { get; set; }
        public string? MovieName { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyCode { get; set; }
        public string? BranchName { get; set; }
        public int? BranchCode { get; set; }
        public string? MoviePreviewImageUrl { get; set; }
        public int? ReleaseYear { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? CompanyTinNumber { get; set; }
        public DateTime? SelectedDate { get; set; }
        public string? PosterUrl { get; set; }
        public string? Overview { get; set; }
        public List<int>? GenreId { get; set; }
        public int? MovieId { get; set; }
        public List<Genre>? Genres { get; set; }
        public List<CastMember>? Cast { get; set; }
        public int? RunTime { get; set; }
        public string? YoutubeKey { get; set; }
        public string? PgRating { get; set; }
        public string? FormattedRunTime
        {
            get
            {
                if (RunTime.HasValue)
                {
                    int hours = RunTime.Value / 60;
                    int minutes = RunTime.Value % 60;
                    return $"{hours}h {minutes}m";
                }
                return null;
            }
        }

        public List<MovieSchedules>? MovieSchedules { get; set; }
    }


    public class Genre
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class CastMember
    {
        public string? Name { get; set; }
        public string? ProfilePath { get; set; }
    }
}
