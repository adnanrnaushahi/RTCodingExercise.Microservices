using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;

namespace Catalog.API.Repositories
{
    public class PlateDetailRepository : IPlateDetailRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public PlateDetailRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //public async Task<PlateDetail> AddPlateDetailAsync(PlateDetail plateDetail)
        //{
        //    var entry = await _dbContext.PlateDetails.AddAsync(plateDetail);
        //    await _dbContext.SaveChangesAsync();
        //    return entry.Entity;
        //}

        //public async Task<PlateDetail?> GetPlateDetailByPlateIdAsync(Guid plateId)
        //{
        //    return await _dbContext.PlateDetails
        //        .FirstOrDefaultAsync(pd => pd.PlateId == plateId);
        //}

        //public async  Task UpdatePlateDetailAsync(PlateDetail plateDetail)
        //{
        //    _dbContext.Entry(plateDetail).State = EntityState.Modified;
        //    await _dbContext.SaveChangesAsync();
        //}
    }
}
