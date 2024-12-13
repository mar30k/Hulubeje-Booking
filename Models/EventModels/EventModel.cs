namespace HulubejeBooking.Models.EventModels
{
    public class EventResponse
    {
        public int? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DoorOpen { get; set; }
        public int? Venue { get; set; }
        public string? VenueCompanyName { get; set; }
        public string? VenueName { get; set; }
        public double? VenueLatitude { get; set; }
        public double? VenueLongitude { get; set; }
        public string? VenuePhone1 { get; set; }
        public string? VenuePhone2 { get; set; }
        public string? VenueSpecificAddress { get; set; }
        public string? VenueAddressLine1 { get; set; }
        public string? VenueAddressLine2 { get; set; }
        public string? VenueAddressLine3 { get; set; }
        public bool? SeatLayout { get; set; }
        public int? Organizer { get; set; }
        public string? OrganizerName { get; set; }
        public string? DefaultImageUrl { get; set; }
        public string? TrailerUrl { get; set; }
        public object? EventSpaces { get; set; }
        public string? Note { get; set; }
        public bool? NeedRegistration { get; set; }
        public int? Type { get; set; }
        public string? TypeName { get; set; }
        public DateTime? TicketClosingDate { get; set; }
        public bool? EveryDayEvent { get; set; }
        public int? Status { get; set; }
        public string? StatusName { get; set; }
        public EventArticles? EventArticles { get; set; }
    }

    public class EventArticles
    {
        public int? Code { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public decimal? PreviousValue { get; set; }
        public string? Description { get; set; }
        public string? DefaultImageUrl { get; set; }
        public List<ChildArticle>? ChildArticles { get; set; }
    }

    public class ChildArticle
    {
        public int? Code { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public decimal? PreviousValue { get; set; }
        public string? Description { get; set; }
        public string? DefaultImageUrl { get; set; }
        public List<ChildArticle>? ChildArticles { get; set; }
    }

}
