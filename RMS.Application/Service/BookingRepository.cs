using RMS.Application.Service.Interface;
using RMS.Domain.Models;
using RMS.Infrastructure.Data;

namespace RMS.Application.Service
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        public readonly ApplicationDbContext _db;
        public BookingRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void update(Booking obj)
        {
            _db.Bookings.Update(obj);
        }
    }
}
