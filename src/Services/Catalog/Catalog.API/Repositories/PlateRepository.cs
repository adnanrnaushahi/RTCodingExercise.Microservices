using Catalog.Domain.Entities;
using Catalog.Domain.Enum;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Models;

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

        public async Task<IEnumerable<Plate>> GetPlatesAsync(int pageSize, int pageIndex, bool orderByAsc = true, PlateStatus? status = null)
        {
            IQueryable<Plate> query = _dbContext.Plates;

            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            query = orderByAsc ? query.OrderBy(p => p.SalePrice) : query.OrderByDescending(p => p.SalePrice);

            return await query.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
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

        public async Task<int> GetTotalPlatesCountAsync(PlateStatus? status = null)
        {
            IQueryable<Plate> query = _dbContext.Plates;

            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            return await query.CountAsync();
        }

        public async Task<(IEnumerable<Plate> Plates, int TotalCount)> GetPlatesByLettersAsync(string letters, int pageSize, int pageIndex)
        {
            if (string.IsNullOrWhiteSpace(letters))
                return (await GetPlatesAsync(pageSize, pageIndex), await GetTotalPlatesCountAsync());

            letters = letters.Trim().ToLower();

            var plates = await _dbContext.Plates
                .Where(p => p.Letters.ToLower().Contains(letters) &&
                    p.Status == Domain.Enum.PlateStatus.Available)
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
                .Where(p => p.Numbers.ToString().Contains(numbers) &&
                p.Status == Domain.Enum.PlateStatus.Available)
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
                .Where(p => (p.Registration.ToLower().Contains(query) ||
                           p.Letters.ToLower().Contains(query) ||
                           p.Numbers.ToString().Contains(query)) &&
                           p.Status == Domain.Enum.PlateStatus.Available)
                .OrderBy(p => p.Registration)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (plates, plates.Count);
        }

        public async Task<decimal> GetTotalSoldRevenueAsync()
        {
            return await _dbContext.Plates.Where(p => p.Status == PlateStatus.Sold).SumAsync(p => p.SalePrice);
        }

        public async Task<Revenue> GetTotalRevenueAsync()
        {
            var soldPlates = await _dbContext.Plates
            .Where(p => p.Status == PlateStatus.Sold)
            .ToListAsync();

            var result = new Revenue
            {
                TotalSalePrice = soldPlates.Sum(p => p.SalePrice),
                AverageProfitMargin = 0
            };

            if (soldPlates.Any())
            {
                decimal vatRate = 0.20m; // 20% VAT rate
                decimal totalProfitMargin = 0;
                decimal totalNetProfit = 0;

                foreach (var plate in soldPlates)
                {
                    // Sale price includes VAT, so remove VAT to get net sale price
                    decimal netSalePrice = plate.SalePrice / (1 + vatRate);

                    // Purchase price is already net (excluding VAT)
                    decimal purchasePrice = plate.PurchasePrice;

                    // Calculate profit
                    decimal profit = netSalePrice - purchasePrice;
                    totalNetProfit += profit;

                    // Calculate profit margin as percentage of purchase price
                    decimal profitMargin = (profit / purchasePrice) * 100;
                    totalProfitMargin += profitMargin;
                }

                result.AverageProfitMargin = Math.Round(totalProfitMargin / soldPlates.Count, 2);
            }
            return result;
        }
    }
}
