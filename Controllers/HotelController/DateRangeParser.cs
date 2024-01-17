using Microsoft.AspNetCore.Mvc;

namespace HulubejeBooking.Controllers.HotelController
{
    public class DateRangeParser
    {
        public static Models.HotelModels.DateRange parseDateRange(string dateRange)
        {
            Models.HotelModels.DateRange result = new Models.HotelModels.DateRange(); // Initialize to a non-null default value

            if (!string.IsNullOrWhiteSpace(dateRange))
            {
                var splited = dateRange?.Split("-");
                if (splited != null && splited.Count() > 1)
                {
                    DateTime startDate, endDate;
                    result.startDate = DateTime.TryParse(splited[0], out startDate) ? (DateTime?)startDate : null;
                    result.endDate = DateTime.TryParse(splited[1], out endDate) ? (DateTime?)endDate : null;
                    result.startDateString = splited[0];
                    result.endDateString = splited[1];
                }
            }

            return result;
        }
    }
}
