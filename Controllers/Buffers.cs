using HulubejeBooking.Controllers.Authentication;
using HulubejeBooking.Models.Authentication;
using System;

namespace HulubejeBooking.Controllers
{
    public class Buffers : IBuffer
    {
        private UserData? _customer;

        public virtual void AddCustomerToBuffer(UserData customer)
        {
            _customer = customer;
        }

        public virtual void DeleteCurrentCustomerFromBuffer()
        {
            _customer = null;
        }

        public virtual async Task<UserData> GetCurrentCustomerFromBuffer()
        {
            return _customer;
        }
    }
}
