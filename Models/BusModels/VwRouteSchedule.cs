namespace HulubejeBooking.Models.BusModels
{
    public class VwRouteSchedule
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int Route { get; set; }

        public int Period { get; set; }

        public int Level { get; set; }

        public int Vehicle { get; set; }

        public int? Driver { get; set; }

        public int? Assistant { get; set; }

        public DateTime DepartureDate { get; set; }

        public DateTime ArrivalDate { get; set; }

        public decimal Tariff { get; set; }

        public string? Note { get; set; }

        public int? LastObjectState { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime LastModifiedOn { get; set; }

        public bool IsActive { get; set; }

        public DateTime? Approved { get; set; }

        public DateTime? Confirmed { get; set; }

        public string? Gate { get; set; }

        public DateTime? VehicleAttended { get; set; }

        public bool Delayed { get; set; }

        public bool Canceled { get; set; }

        public int? VehicleChangedTo { get; set; }

        public bool CrewAttended { get; set; }

        public bool PassengersBoarded { get; set; }

        public DateTime? VehicleDeparted { get; set; }

        public string? Remark { get; set; }

        public string PeriodName { get; set; } = null!;

        public string RouteName { get; set; } = null!;

        public string? RouteDescription { get; set; }

        public string Code { get; set; } = null!;

        public string? DriverName { get; set; }

        public string? AssistantName { get; set; }

        public string LevelDesc { get; set; } = null!;

        public string? LastObjectStateDesc { get; set; }

        public int VehicleId { get; set; }

        public int RegionCode { get; set; }

        public string RegionCodeDesc { get; set; } = null!;

        public int PlateCode { get; set; }

        public string PlateCodeDesc { get; set; } = null!;

        public string PlateNumber { get; set; } = null!;

        public int SideNumber { get; set; }

        public int VehicleOperatorId { get; set; }

        public string? VehicleOperator { get; set; }

        public int? ChangerVehicleId { get; set; }

        public int? ChangerRegionCode { get; set; }

        public string? ChangerRegionCodeDesc { get; set; }

        public int? ChangerPlateCode { get; set; }

        public string? ChangerPlateCodeDesc { get; set; }

        public string? ChangerPlateNumber { get; set; }

        public int? ChangerSideNumber { get; set; }

        public int? ChangerOperatorId { get; set; }

        public string? ChangerOperator { get; set; }

        public decimal? TravelDuration { get; set; }
        public int? Via { get; set; }
        public int? Demand { get; set; }
        public int? Distance { get; set; }
        public int? OriginTerminal { get; set; }
        public string? ImgUrl { get; set; }
        public List<string>? Amenities { get; set; }
        public string? VehiclePlateNumber { get; set; }
        public string? OriginTerminalName { get; set; }
        public string? ViaDescription { get; set; }
        public string? DestiantionCity { get; set; }
        public string? DepatureCity { get; set; }

    }
}
