namespace HulubejeBooking.Models.HotelModels
{
    public class FilteredHotel
    {
        public int adultCount { get; set; }
        public int childCount { get; set; }                                
        public int roomCount { get; set; }
        public int numberOfDay { get; set; }
        public string departureDate { get; set; }
        public string arrivalDate { get; set; }
        public string cityName { get; set; }

        public List<FilteredCompany> filteredCompanies { get; set; }
    }

    public class FilteredCompany
    {
        public bool isSponsored { get; set; }
        public string code { get; set; }
        public string tradeName { get; set; }
        public string brandName { get; set; }
        public string industryType { get; set; }
        public double rating { get; set; }
        public string TIN { get; set; }
        public List<string> attachments { get; set; }
        public string registerDate { get; set; }
        public bool isTaxInclusive { get; set; }
        public string termsAndConditionUrl { get; set; }
        public int ratingCount { get; set; }
        public string oud { get; set; }
        public string branchName { get; set; }
        public string branchCategory { get; set; }
    }

}
