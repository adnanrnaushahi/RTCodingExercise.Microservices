using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;

namespace Catalog.API.Repositories
{
    public class StatusChangeLogRepository : IStatusChangeLogRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public StatusChangeLogRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<StatusChangeLog> AddLogEntryAsync(StatusChangeLog logEntry)
        {
            var entry = await _dbContext.StatusChangeLogs.AddAsync(logEntry);
            await _dbContext.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<IEnumerable<StatusChangeLog>> GetLogEntriesForPlateAsync(Guid plateId)
        {
            return await _dbContext.StatusChangeLogs
                .Where(log => log.PlateId == plateId)
                .OrderByDescending(log => log.ChangedAt)
                .ToListAsync();
        }
    }
}
