using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;

namespace Catalog.API.Repositories
{
    public class PlateRepository : IPlateRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public PlateRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Plate> AddPlateAsync(Plate plate)
        {
            var entry = await _dbContext.Plates.AddAsync(plate);
            await _dbContext.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task DeletePlateAsync(Guid id)
        {
            var plate = await _dbContext.Plates.FindAsync(id);
            if (plate != null)
            {
                _dbContext.Plates.Remove(plate);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Plate>> GetPlatesAsync(int pageSize, int pageIndex, bool orderByAsc = true)
        {
            return orderByAsc ? await _dbContext.Plates.OrderBy(p => p.SalePrice).Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync()
                : await _dbContext.Plates.OrderByDescending(p => p.SalePrice).Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Plate?> GetPlateByIdAsync(Guid id)
        {
            return await _dbContext.Plates.FindAsync(id);
        }

        public async Task UpdatePlateAsync(Plate plate)
        {
            _dbContext.Entry(plate).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> GetTotalPlatesCountAsync()
        {
            return await _dbContext.Plates.CountAsync();
        }
       
    }
}
