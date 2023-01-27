using Booking.Core.Entities;

namespace Booking.Data.Repositories
{
    public interface IGymClassRepository
    {
        Task<List<GymClass>> GetAsync();
    }
}