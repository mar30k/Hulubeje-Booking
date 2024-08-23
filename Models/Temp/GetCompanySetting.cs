namespace HulubejeBooking.Models.Temp
{
    public class GetCompanySetting
    {
        public bool RecieveOrderOnClosingHours { get; set; }
        public int? MinimumStock { get; set; }
        public int? ControlStock { get; set; }
        public DateTime? CheckinTime { get; set; }
        public DateTime? CheckoutTime { get; set; }
        public string? CancilationPolicy { get; set; }
        public bool? IsEnabledDelivery { get; set; }
        public bool? IsEnabledPickupAtBranch { get; set; }
        public bool IsTaxInclusive { get; set; }
        public DateTime ServerTime { get; set; }
    }

    public class GetCompany
    {
        public List<object>? Tables { get; set; }
        public List<Occasion>? Occasions { get; set; }
        public int ReservationFee { get; set; } 
        public DateTime ServerTime { get; set; }
    }

    public class Occasion
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class ReserveTable
    {
        public HulubejeResponse<GetCompany>? GetCompany { get; set; }
        public HulubejeResponse<GetCompanySetting>? GetCompanySetting { get; set; }
    }

    public class ReservationModel
    {
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public int NumberOfGuests { get; set; }
        public int Duration { get; set; }
        public int OccasionId { get; set; }
        public string? SpecialRequirements { get; set; }
        public decimal ReservationFee { get; set; }

        // Constructor
        public ReservationModel()
        {
            Date = DateTime.Now.Date;
            Time = DateTime.Now;
            NumberOfGuests = 1;
            SpecialRequirements = string.Empty;
        }
    }
}
