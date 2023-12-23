namespace HulubejeBooking.Models.HotelModels
{
    public class Payment
    {
        public int? OperationMode { get; set; }
        public string? PaymentOptionName { get; set; }
        public string? PaymentProviderOrgUnitDef { get; set; }
        public string? PaymentProviderOrganizationTin { get; set; }
        public string? PaymentProviderOrganizationName { get; set; }
        public string? PaymentMethodImage { get; set; }
        public string? PaymentProviderImage { get; set; }
    }
}
