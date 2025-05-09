using Catalog.Domain.Entities;
using Catalog.Domain.Interfaces;
using IntegrationEvents.Events;
using MassTransit;

namespace Catalog.API.EventBus.Consumers
{
    public class PlateStatusChangedConsumer : IConsumer<PlateStatusChangedEvent>
    {
        private readonly ILogger<PlateStatusChangedConsumer> _logger;
        private readonly IStatusChangeLogRepository _statusChangeLogRepository;

        public PlateStatusChangedConsumer(ILogger<PlateStatusChangedConsumer> logger, IStatusChangeLogRepository statusChangeLogRepository)
        {
            _logger = logger;
            _statusChangeLogRepository = statusChangeLogRepository;
        }

        public async Task Consume(ConsumeContext<PlateStatusChangedEvent> context)
        {
            var message = context.Message;

            try
            {
                // Save to repository
                await _statusChangeLogRepository.AddLogEntryAsync(new StatusChangeLog(
                    message.PlateId,
                    message.Registration,
                    message.OldStatus,
                    message.NewStatus));

                _logger.LogInformation("Processing PlateStatusChangedEvent: {PlateId}, {OldStatus} -> {NewStatus}", message.PlateId, message.OldStatus, message.NewStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PlateStatusChangedEvent: {PlateId}", message.PlateId);
                throw;

            }
        }
    }
}
