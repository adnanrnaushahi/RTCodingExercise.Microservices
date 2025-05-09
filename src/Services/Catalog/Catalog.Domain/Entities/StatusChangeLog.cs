using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog.Domain.Enum;

namespace Catalog.Domain.Entities
{
    public class StatusChangeLog
    {
        public Guid Id { get; private set; }
        public Guid PlateId { get; private set; }
        public string Registration { get; private set; }
        public PlateStatus OldStatus { get; private set; }
        public PlateStatus NewStatus { get; private set; }
        public string ChangedBy { get; private set; }
        public DateTime ChangedAt { get; private set; }

        public StatusChangeLog(Guid plateId, string registration, PlateStatus oldStatus, PlateStatus newStatus)
        {
            Id = Guid.NewGuid();
            PlateId = plateId;
            Registration = registration;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            ChangedBy = "System";
            ChangedAt = DateTime.UtcNow;
        }
    }
}
