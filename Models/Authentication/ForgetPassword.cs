namespace HulubejeBooking.Models.Authentication
{
    public class ForgetPassword
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public int Type { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Nationality { get; set; }
        public DateTime DOB { get; set; }
        public string Title { get; set; }
        public string Gender { get; set; }
        public string Religion { get; set; }
        public string Preference { get; set; }
        public bool IsActive { get; set; }
        public string Remark { get; set; }
    }
}
