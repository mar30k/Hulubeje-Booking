using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.Authentication;
using System.Security.Cryptography;

namespace HulubejeBooking.Controllers
{
    public interface IBuffer
    {
        void AddCustomerToBuffer(UserData customer);
        void DeleteCurrentCustomerFromBuffer();
        Task<UserData> GetCurrentCustomerFromBuffer();
    }
}
