using Microsoft.AspNetCore.Mvc;

namespace HulubejeBooking.Controllers.HotelController
{
    public class GuestInfoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GuestInfo(string roomTypecode, string orgTin, string Date, int adultCount, int childCount, int roomCount, string oud)
        {

            return View();
        }
    }
}
