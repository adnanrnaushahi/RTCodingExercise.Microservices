using Catalog.API.Repositories;
using Catalog.Domain.Entities;
using Catalog.Domain.Enum;
using Catalog.Domain.Interfaces;
using IntegrationEvents.Events;
using MassTransit;
namespace Catalog.API.Services
{
    public class PlateService : IPlateService
    {
        private readonly IPlateRepository _plateRepository;
        private readonly IStatusChangeLogRepository _statusChangeLogRepository;
        private readonly IBus _bus;

        public PlateService(
        IPlateRepository plateRepository,
        IStatusChangeLogRepository statusChangeLogRepository,
        IBus bus)
        {
            _plateRepository = plateRepository;
            _statusChangeLogRepository = statusChangeLogRepository;
            _bus = bus;
        }

        public async Task<Plate> CreatePlateAsync(string registration, decimal purchasePrice, decimal salePrice, string letters, int numbers)
        {
            var plate = new Plate(registration, purchasePrice, salePrice, letters, numbers);
            return await _plateRepository.AddPlateAsync(plate);
        }

        public async Task<Plate?> GetPlateByIdAsync(Guid id)
        {
            return await _plateRepository.GetPlateByIdAsync(id);
        }

        public async Task<(IEnumerable<Plate> Plates, int TotalCount)> GetPlatesAsync(int pageSize, int pageIndex, bool orderByAsc = true)
        {
            var plates = await _plateRepository.GetPlatesAsync(pageSize, pageIndex, orderByAsc);
            var totalCount = await _plateRepository.GetTotalPlatesCountAsync();
            return (plates, totalCount);
        }

        public async Task<(IEnumerable<Plate> Plates, int TotalCount)> GetPlatesByLettersAsync(string letters, int pageSize, int pageIndex)
        {
            return await _plateRepository.GetPlatesByLettersAsync(letters, pageSize, pageIndex);
        }

        public async Task<(IEnumerable<Plate> Plates, int TotalCount)> GetPlatesByNumbersAsync(string numbers, int pageSize, int pageIndex)
        {
            return await _plateRepository.GetPlatesByNumbersAsync(numbers, pageSize, pageIndex);            
        }

        public async Task<(IEnumerable<Plate> Plates, int TotalCount)> SearchPlatesAsync(string query, int pageSize, int pageIndex)
        {
            return await _plateRepository.SearchPlatesAsync(query, pageSize, pageIndex);
        }
        public async Task<Plate> UpdatePlateStatusAsync(Guid plateId, PlateStatus newStatus)
        {
            var plate = await _plateRepository.GetPlateByIdAsync(plateId);

            if (plate == null)
                throw new KeyNotFoundException($"Plate with ID {plateId} not found");

            var oldStatus = plate.Status;

            if (oldStatus != newStatus)
            {
                plate.Status = newStatus;
                await _plateRepository.UpdatePlateAsync(plate);

                // Create and publish the event
                var statusChangedEvent = new PlateStatusChangedEvent(
                    plateId,
                    plate.Registration,
                    oldStatus,
                    newStatus);

                await _bus.Publish(statusChangedEvent);
            }

            return plate;
        }
    }
}
