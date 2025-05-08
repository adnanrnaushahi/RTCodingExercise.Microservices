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
        
        public async Task<(IEnumerable<Plate> Plates, int TotalCount)> GetPlatesByLettersAsync(string letters, int pageSize, int pageIndex)
        {
            if (string.IsNullOrWhiteSpace(letters))
                return (await GetPlatesAsync(pageSize, pageIndex), await GetTotalPlatesCountAsync());

            letters = letters.Trim().ToLower();

            var plates = await _dbContext.Plates
                .Where(p => p.Letters.ToLower().Contains(letters))
                .OrderBy(p => p.Registration)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (plates, plates.Count);
        }

        public async Task<(IEnumerable<Plate> Plates, int TotalCount)> GetPlatesByNumbersAsync(string numbers, int pageSize, int pageIndex)
        {
            if (string.IsNullOrWhiteSpace(numbers))
                return (await GetPlatesAsync(pageSize, pageIndex), await GetTotalPlatesCountAsync());

            var plates = await _dbContext.Plates
                .Where(p => p.Numbers.ToString().Contains(numbers))
                .OrderBy(p => p.Registration)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (plates, plates.Count);
        }
       

        public async Task<(IEnumerable<Plate> Plates, int TotalCount)> SearchPlatesAsync(string query, int pageSize, int pageIndex)
        {
            if (string.IsNullOrWhiteSpace(query))
                return (await GetPlatesAsync(pageSize, pageIndex), await GetTotalPlatesCountAsync());

            query = query.Trim().ToLower();

            var plates = await _dbContext.Plates
                .Where(p => p.Registration.ToLower().Contains(query) ||
                           p.Letters.ToLower().Contains(query) ||
                           p.Numbers.ToString().Contains(query))
                .OrderBy(p => p.Registration)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (plates, plates.Count);
        }
    }
}
