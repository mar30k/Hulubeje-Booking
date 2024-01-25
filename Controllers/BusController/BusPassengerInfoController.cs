using HulubejeBooking.Models.BusModels;
using Microsoft.AspNetCore.Mvc;

namespace HulubejeBooking.Controllers.BusController
{
    public class BusPassengerInfoController : Controller
    {
        public IActionResult PassengerInfo(int selectedButtonsCount, List<string> seatName, string plateNumber, string terminal, int distance, List<string> seatId, int vehicleOperatorId, int routeScheduleId,
            string tariff, string level, string route, string operatorName, string scheduleDate, DateTime scheduleTime, string destinationCity, string depatureCity, DateTime arrivialDate, DateTime departureDate)
        {
            var schedule = new VwRouteSchedule()
            {
                Id = routeScheduleId,
                PlateNumber = plateNumber,
                Terminal = terminal,
                Distance = distance,
                Amount = tariff,
                LevelDesc = level,
                OperatorName = operatorName,
                RouteDescription = route,
                Date = scheduleDate,
                Time = scheduleTime,
                DestCityName = destinationCity,
                DepatureCity = depatureCity,
                ArrivalDate = arrivialDate,
                DepartureDate = departureDate,
                SeatId = seatId,
                VehicleOperatorId = vehicleOperatorId,
                NoOfSeat = selectedButtonsCount,
                SeatName = seatName,
            };
          return View(schedule);
        }
    }
}
