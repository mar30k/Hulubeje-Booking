namespace HulubejeBooking.Models.PaymentModels
{
    public class PaymentOptionModel
    {
        public int? operationMode { get; set; }
        public string? paymentOptionName { get; set; }
        public string? paymentProviderOrgUnitDef { get; set; }
        public string? paymentProviderOrganizationTin { get; set; }
        public string? paymentProviderOrganizationName { get; set; }
        public string? paymentProviderImage { get; set; }
        public string? paymentOrganizationImage { get; set; }
    }
    public class BoAModel 
    {
        public int? operationMode { get; set; } = 0;
        public string? paymentOptionName { get; set; } = "BOA Card Payment";
        public string? paymentProviderOrgUnitDef { get; set; } = "0000006979-01";
        public string? paymentProviderOrganizationTin { get; set; } = "0000006979";
        public string? paymentProviderOrganizationName { get; set; } = "Pay With Card";
        public string? paymentProviderImage { get; set; } = "https://api-hulubeje.cnetcommerce.com/assets/LOG-02.png";
        public string? paymentOrganizationImage { get; set; } = "https://api-hulubeje.cnetcommerce.com/assets/LOG-01.png";
    }
    public class Wrapper
    {
        public List<PaymentOptionModel>? PaymentOptions { get; set; }
        public BoAModel? Boa { get; set; }
    }
}
