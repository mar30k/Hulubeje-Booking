using HulubejeBooking.Models.Authentication;

namespace HulubejeBooking.Models
{
	public class Images
	{
		public List<string>? Hotel { get; set; }
		public List<string>? Cinema { get; set; }
		public List<string>? Event { get; set; }
		public List<string>? Bus { get; set; }
		public List<string>? Spa { get; set; }
		public CookieValidation? CookieValidation { get; set; }
	}
}
