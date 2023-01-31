using Booking.Core.Entities;
using Booking.Core.Repositories;
using Booking.Data.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Data.Repositories
{
    public class GymClassRepository : IGymClassRepository
    {
        private readonly ApplicationDbContext db;

        public GymClassRepository(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<List<GymClass>> GetAsync()
        {
            return await db.GymClasses.ToListAsync();
        }

        public async Task<IEnumerable<GymClass>> GetWithAttendinAsync()
        {
            return await db.GymClasses.Include(g => g.AttendingMembers).OrderByDescending(g => g.StartTime).ToListAsync();
        }
        
        public async Task<GymClass?> GetAsync(int id)
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            return await db.GymClasses.FirstOrDefaultAsync(m => m.Id == id);
        }

        public void Add(GymClass gymclass)
        {
            db.Add(gymclass);
        }

        public async Task<IEnumerable<GymClass>> GetHistoryAsync()
        {
            return await db.GymClasses
                        .Include(g => g.AttendingMembers)
                        .IgnoreQueryFilters()
                        .Where(g => g.StartTime < DateTime.Now)
                        .OrderByDescending(g => g.StartTime)
                        .ToListAsync();
        }
    }


    public class MockRepo : IGymClassRepository
    {
        private ApplicationDbContext db;

        public MockRepo(ApplicationDbContext db)
        {
            this.db = db;
        }

        public void Add(GymClass gymclass)
        {
            throw new NotImplementedException();
        }

        public Task<List<GymClass>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public Task<GymClass?> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GymClass>> GetHistoryAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GymClass>> GetWithAttendinAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class MockUoW : IUnitOfWork
    {
        public IGymClassRepository GymClassRepository
        {
            get;
        }
        public IApplicationUserGymClassRepository ApplicationUserGymClassRepository
        {
            get;
        }

        public Task CompleteAsync()
        {
            throw new NotImplementedException();
        }
    }
}
