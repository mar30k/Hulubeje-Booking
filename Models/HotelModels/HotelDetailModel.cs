namespace HulubejeBooking.Models.HotelModels
{
	public class PaymentOption
    {
        public string? PaymentMethodName { get; set; }
        public string? PaymentMethodOud { get; set; }
        public string? PaymentMethodImage { get; set; }
    }

    //public class Amenity
    //{
    //    public int? Index { get; set; }
    //    public string? Code { get; set; }
    //    public string? Name { get; set; }
    //    public string? ImageUrl { get; set; }
    //    public string? Description { get; set; }
    //}

    public class HotelDetailModel
    {
        public string? Name { get; set; }
        public string? branchCode { get; set; }
        public string? orgTin { get; set; }
        public string? note { get; set; }
        public List<object>? schedule { get; set; } 
        public List<PaymentOption>? paymentOptions { get; set; }
        public List<object>? deliveryMethods { get; set; } 
        public decimal? averageRating { get; set; }
        public int? totalRatings { get; set; }
        public int? hotelRating { get; set; }
        public string? state { get; set; }
        public List<Amenity>? aminities { get; set; }
        public List<string>? Images { get; set; }
    }
}
