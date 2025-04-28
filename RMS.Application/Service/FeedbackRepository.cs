using RMS.Application.Service.Interface;
using RMS.Domain.Models;
using RMS.Infrastructure.Data;

namespace RMS.Application.Service
{
    public class FeedbackRepository : Repository<Feedback>, IFeedBackRepository
    {
        private readonly ApplicationDbContext _db;
        public FeedbackRepository(ApplicationDbContext db) : base(db) 
        { 
            _db = db;     
        }
        public void Update(Feedback feedback)
        {   
           _db.Feedbacks.Update(feedback);
        }
    }
}
