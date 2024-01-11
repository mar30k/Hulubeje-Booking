namespace HulubejeBooking.Models.PaymentModels.CinemaPaymentModel
{
    public class CinemaDetails
    {
        public string Consignee { get; set; }
        public string GrandTotal { get; set; }
        public string MovieSchedule { get; set; }
        public List<string> SeatsToBook { get; set; }
        public string OrgUnitDef { get; set; }
        public string OrgTin { get; set; }
        public string TransactionReference { get; set; }
        public List<CinemaArticle> CinemaArticles { get; set; }
        public OnBookSuccess OnBookSuccess { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Platform { get; set; }
        public PaymentInfoModel PaymentInfo { get; set; }
    }
    public class CinemaArticle
    {
        public string Code { get; set; }
        public double TaxAmount { get; set; }
        public double TotalAmount { get; set; }
        public double TaxableAmount { get; set; }
        public int TaxType { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string LineItemNote { get; set; }
        public string SpecialFlag { get; set; }
    }

    public class OnBookSuccess
    {
        public string FirstName { get; set; }
        public List<string> Seats { get; set; }
        public List<string> Items { get; set; }
        public string MovieName { get; set; }
        public string MovieDimension { get; set; }
        public string HallName { get; set; }
        public string Company { get; set; }
        public string Time { get; set; }
        public string Date { get; set; }
    }
}
