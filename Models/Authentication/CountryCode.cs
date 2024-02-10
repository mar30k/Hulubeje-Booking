namespace HulubejeBooking.Models.Authentication
{
    public class CountryResponse
    {
        public Flags? Flags { get; set; } 
        public Idd? Idd { get; set; }
        public Names? Name { get; set; }
    }
    public class Idd
    {
        public string? Root { get; set; }
        public List<string>? Suffixes { get; set; }
    }
    public class Names
    {
        public string? Common { get; set; }
    }
    public class Flags
    {
        public string? Png { get; set; } 
        public string? Svg { get; set; } 
        public string? Alt { get; set; } 
    }
}
