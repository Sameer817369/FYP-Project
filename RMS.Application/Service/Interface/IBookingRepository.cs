using RMS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMS.Application.Service.Interface
{
    public interface IBookingRepository : IRepository<Booking>
    {
        void update(Booking obj);
    }
}
