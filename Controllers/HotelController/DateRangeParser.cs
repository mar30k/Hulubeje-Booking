using Microsoft.AspNetCore.Mvc;

namespace HulubejeBooking.Controllers.HotelController
{
    public class DateRangeParser
    {
        public static Models.HotelModels.DateRange parseDateRange(string dateRange)
        {
            Models.HotelModels.DateRange result = new();

            if (!string.IsNullOrWhiteSpace(dateRange))
            {
                var splited = dateRange?.Split("-");
                if (splited != null && splited.Length > 1)
                {
                    result.startDate = DateTime.TryParse(splited[0], out DateTime startDate) ? (DateTime?)startDate : null;
                    result.endDate = DateTime.TryParse(splited[1], out DateTime endDate) ? (DateTime?)endDate : null;
                    result.startDateString = splited[0];
                    result.endDateString = splited[1];
                }
            }  

            return result;
        }
    }
}
