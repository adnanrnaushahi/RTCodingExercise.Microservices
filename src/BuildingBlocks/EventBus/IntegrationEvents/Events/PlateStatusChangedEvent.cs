using System.ComponentModel.DataAnnotations;
using Catalog.Domain.Enum;

namespace IntegrationEvents.Events
{
    public class PlateStatusChangedEvent : IntegrationEvent
    {
        public Guid PlateId { get; }
        public string Registration { get; }
        public PlateStatus OldStatus { get; }
        public PlateStatus NewStatus { get; }

        public PlateStatusChangedEvent(Guid plateId, string registration, PlateStatus oldStatus,
                                     PlateStatus newStatus)
        {
            PlateId = plateId;
            Registration = registration;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            CorrelationId = Guid.NewGuid();
        }
    }
}
