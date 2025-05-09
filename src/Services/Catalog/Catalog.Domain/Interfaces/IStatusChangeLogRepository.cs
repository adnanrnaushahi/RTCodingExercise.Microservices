using Catalog.Domain.Entities;

namespace Catalog.Domain.Interfaces
{
    public interface IStatusChangeLogRepository
    {
        Task<StatusChangeLog> AddLogEntryAsync(StatusChangeLog logEntry);
        Task<IEnumerable<StatusChangeLog>> GetLogEntriesForPlateAsync(Guid plateId);
    }
}
