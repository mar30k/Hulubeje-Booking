namespace HulubejeBooking.Models.PaymentModels.BusPaymentModel
{
    public class PassengerInfo
    {
        public int? Operator { get; set; }
        public int? Agent { get; set; }
        public List<TicketDetail>? TicketDetail { get; set; }
        public int? RouteSchedule { get; set; }
        public DateTime RouteScheduleDate { get; set; }
        public int? PaymentMethod { get; set; }
        public int? PaymentProcessor { get; set; }
        public string? Payer { get; set; }
        public string? PaymentRefNumber { get; set; }
        public decimal? PaymentAmount { get; set; }
        public DateTime? MaturityDate { get; set; }
        public DateTime? PaymentIssueDate { get; set; }
        public int? PaymentStatus { get; set; }
        public string? IpAddress { get; set; }
        public string? Platform { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? AppId { get; set; }
    }
    public class TicketDetail
    {
        public string? Pnr { get; set; }
        public string? IdNumber { get; set; }
        public string? NationalId { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public int? Gender { get; set; }
        public int? MaritalStatus { get; set; }
        public DateTime? Dob { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmergencyContact { get; set; }
        public string? Email { get; set; }
        public int? Region { get; set; }
        public int? City { get; set; }
        public int? SubCity { get; set; }
        public string? Woreda { get; set; }
        public string? HouseNumber { get; set; }
        public string? SpecificAddress { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public string? ImageUrl { get; set; }
        public int? SeatLayout { get; set; }
        public bool? IsAutoAssigned { get; set; }
        public int? PickupLocation { get; set; }
        public string? BillToTin { get; set; }
        public string? Note { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? Discount { get; set; }
        public decimal? AdditionalCharge { get; set; }
        public decimal? GrandTotal { get; set; }
    }
}
