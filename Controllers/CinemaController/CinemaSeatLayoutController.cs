using HulubejeBooking.Models.CInemaModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace CinemaSeatBooking.Controllers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class CinemaSeatLayoutController : Controller
{
    private readonly HttpClient _httpClient;
    public CinemaSeatLayoutController()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api-hulubeje.cnetcommerce.com/api/")
        };
    }
    public async Task<IActionResult> SeatArrangement(string spacecode, string companyTinNumber, string branchCode, string companyName, string movieName, string movieCode, string dimension, string spaceType, string selectedDate, string code, decimal price, string hallName, string utcTime)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"cinema/getCinemaSeatArrangment?orgTin={companyTinNumber}&spaceCode={spacecode}");

        if (response.IsSuccessStatusCode)
        {
            string responseData = await response.Content.ReadAsStringAsync();

            // Deserialize into a single SeatLayout instance
            var seatArrangement = JsonConvert.DeserializeObject<SeatLayout>(responseData);

            // Make a second API call to get booked seats
            HttpResponseMessage bookedSeatsResponse = await _httpClient.GetAsync($"cinema/GetBookedSeats?orgTin={companyTinNumber}&scheduleCode={code}&spaceCode={spacecode}");

            if (bookedSeatsResponse.IsSuccessStatusCode)
            {
                string bookedSeatsData = await bookedSeatsResponse.Content.ReadAsStringAsync();

                // Deserialize the response into a List<string>
                var bookedSeats = JsonConvert.DeserializeObject<List<string>>(bookedSeatsData);

                // Update the SeatLayout model with the booked seats information
                if (seatArrangement is not null)
                {
                    seatArrangement.BookedSeats ??= bookedSeats;
                }
            }
            else
            {
                // Handle the case when the second API call fails
                return View("Error");
            }

            HttpResponseMessage soldseatsResponse = await _httpClient.GetAsync($"cinema/GetSoldSeats?orgTin={companyTinNumber}&scheduleCode={code}&spaceCode={spacecode}");
            if (soldseatsResponse.IsSuccessStatusCode)
            {
                string soldSeatData = await soldseatsResponse.Content.ReadAsStringAsync();

                var soldSeats = JsonConvert.DeserializeObject<List<string>>(soldSeatData);
                if (seatArrangement is not null)
                {
                    seatArrangement.SoldSeats ??= soldSeats;
                }
            }
            else
            {
                //handle the case where GetSoldSeats api fails 
                return View("Error");
            }

            HttpResponseMessage availableSeatsResponse = await _httpClient.GetAsync($"cinema/GetAvailableSeats?orgTin={companyTinNumber}&scheduleCode={code}&spaceCode={spacecode}");
            if (availableSeatsResponse.IsSuccessStatusCode)
            {
                string availableSeatsData = await availableSeatsResponse.Content.ReadAsStringAsync();
                var availableSeats = JsonConvert.DeserializeObject<List<string>>(availableSeatsData);
                if (seatArrangement is not null)
                {
                    seatArrangement.AvailableSeats ??= availableSeats;
                }
                var anonymousObject = new
                {
                    schedule = code,
                    allSeats = seatArrangement?.AvailableSeats,
                    orgTin = companyTinNumber,
                };

                string jsonBody = JsonConvert.SerializeObject(anonymousObject);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                HttpResponseMessage takenSeatsResponse = await _httpClient.PostAsync("ManageSeatCache/GetCachedSeatsForSchedule", content);
                if (takenSeatsResponse.IsSuccessStatusCode)
                {
                    string takenSeatsResponseData = await takenSeatsResponse.Content.ReadAsStringAsync();
                    var takenSeats = JsonConvert.DeserializeObject<List<string>>(takenSeatsResponseData);
                    if (seatArrangement is not null)
                    {
                        seatArrangement.TakenSeats ??= takenSeats;
                    }
                }
                else
                {
                    return View("Error");
                }
            }
            if (seatArrangement is not null)
            {
                seatArrangement.CompanyTinNumber = companyTinNumber;
                seatArrangement.BranchCode = branchCode;
                seatArrangement.SpaceCode = spacecode;
                seatArrangement.MovieScheduleCode = code;
                seatArrangement.Price = price;
                seatArrangement.SelectedDate = selectedDate;
                seatArrangement.HallName = hallName;
                seatArrangement.UtcTime = utcTime;
                seatArrangement.CompanyName = companyName;
                seatArrangement.MovieName = movieName;
                seatArrangement.Dimension = dimension;
                seatArrangement.SpaceType = spaceType;
                seatArrangement.ArticleCode = movieCode;
            }
            // Pass the updated SeatLayout instance to the view
            return View(seatArrangement);
        }

        // Handle the case when the first API call fails
        return View("Error");
    }
    public async Task<IActionResult> GetUpdatedSeatInfo(string spacecode, string companyTinNumber, string code, string movieName, string companyName, string utcTime, string selectedDate, string hallName,
        string spaceType, string dimension)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"cinema/getCinemaSeatArrangment?orgTin={companyTinNumber}&spaceCode={spacecode}");

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();

                var updatedSeatModel = JsonConvert.DeserializeObject<SeatLayout>(responseData);

                HttpResponseMessage bookedSeatsResponse = await _httpClient.GetAsync($"cinema/GetBookedSeats?orgTin={companyTinNumber}&scheduleCode={code}&spaceCode={spacecode}");

                if (bookedSeatsResponse.IsSuccessStatusCode)
                {
                    string bookedSeatsData = await bookedSeatsResponse.Content.ReadAsStringAsync();
                    var bookedSeats = JsonConvert.DeserializeObject<List<string>>(bookedSeatsData);
                    if (updatedSeatModel is not null)
                    {
                        updatedSeatModel.BookedSeats ??= bookedSeats;
                    }
                }
                else
                {
                    return PartialView("_SeatArrangementPartialView", null);
                }

                HttpResponseMessage soldseatsResponse = await _httpClient.GetAsync($"cinema/GetSoldSeats?orgTin={companyTinNumber}&scheduleCode={code}&spaceCode={spacecode}");
                if (soldseatsResponse.IsSuccessStatusCode)
                {
                    string soldSeatData = await soldseatsResponse.Content.ReadAsStringAsync();
                    var soldSeats = JsonConvert.DeserializeObject<List<string>>(soldSeatData);
                    if (updatedSeatModel is not null)
                    {
                        updatedSeatModel.SoldSeats ??= soldSeats;
                    }
                }
                else
                {
                    return PartialView("_SeatArrangementPartialView", null);
                }

                HttpResponseMessage availableSeatsResponse = await _httpClient.GetAsync($"cinema/GetAvailableSeats?orgTin={companyTinNumber}&scheduleCode={code}&spaceCode={spacecode}");
                if (availableSeatsResponse.IsSuccessStatusCode)
                {
                    string availableSeatsData = await availableSeatsResponse.Content.ReadAsStringAsync();
                    var availableSeats = JsonConvert.DeserializeObject<List<string>>(availableSeatsData);
                    if (updatedSeatModel is not null)
                    {
                        updatedSeatModel.AvailableSeats ??= availableSeats;
                    }
                    var anonymousObject = new
                    {
                        schedule = code,
                        allSeats = updatedSeatModel?.AvailableSeats,
                        orgTin = companyTinNumber
                    };

                    string jsonBody = JsonConvert.SerializeObject(anonymousObject);
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    HttpResponseMessage takenSeatsResponse = await _httpClient.PostAsync("ManageSeatCache/GetCachedSeatsForSchedule", content);
                    if (takenSeatsResponse.IsSuccessStatusCode)
                    {
                        string takenSeatsResponseData = await takenSeatsResponse.Content.ReadAsStringAsync();
                        var takenSeats = JsonConvert.DeserializeObject<List<string>>(takenSeatsResponseData);
                        if (updatedSeatModel is not null)
                        {
                            updatedSeatModel.TakenSeats ??= takenSeats;
                        }
                    }
                    else
                    {
                        return PartialView("_SeatArrangementPartialView", null);
                    }
                    updatedSeatModel.CompanyName = companyName;
                    updatedSeatModel.Dimension = dimension; 
                    updatedSeatModel.MovieName = movieName;
                    updatedSeatModel.UtcTime = utcTime;
                    updatedSeatModel.SelectedDate = selectedDate;
                    updatedSeatModel.SpaceType = spaceType; 
                    updatedSeatModel.HallName = hallName;
                }
                return PartialView("_SeatArrangementPartialView", updatedSeatModel);
            }
            else
            {
                return PartialView("_SeatArrangementPartialView", null);
            }
        }
        catch (Exception)
        {
            return PartialView("_SeatArrangementPartialView", null);
        }
    }

}
