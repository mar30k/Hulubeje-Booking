
namespace HulubejeBooking.Models.HotelModels
{
    public class DateRange
    {
        internal DateTime? startDate;
        internal DateTime? endDate;
        internal string startDateString;
        internal string endDateString;

        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string StartDateString { get; set; }
        public string EndDateString { get; set; }
    }
}
