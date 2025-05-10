using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.API.Services;
using Catalog.Domain.Entities;
using Catalog.Domain.Enum;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Models;
using IntegrationEvents.Events;
using MassTransit;
using Moq;
using Xunit;

namespace Catalog.UnitTests
{
    public class PlateServiceTests
    {
        private readonly Mock<IPlateRepository> _plateRepositoryMock;
        private readonly Mock<IStatusChangeLogRepository> _statusChangeLogRepositoryMock;
        private readonly Mock<IBus> _busMock;
        private readonly IPlateService _plateService;

        public PlateServiceTests()
        {
            _plateRepositoryMock = new Mock<IPlateRepository>();
            _statusChangeLogRepositoryMock = new Mock<IStatusChangeLogRepository>();
            _busMock = new Mock<IBus>();
            _plateService = new PlateService(_plateRepositoryMock.Object, _statusChangeLogRepositoryMock.Object, _busMock.Object);
        }

        [Fact]
        public async Task CreatePlateAsync_ShouldAddAndReturnPlate()
        {
            // Arrange
            var plate = new Plate("REG123", 1000, 1500, "ABC", 123);
            _plateRepositoryMock.Setup(r => r.AddPlateAsync(It.IsAny<Plate>())).ReturnsAsync(plate);

            // Act
            var result = await _plateService.CreatePlateAsync("REG123", 1000, 1500, "ABC", 123);

            // Assert
            Assert.Equal(plate, result);
            _plateRepositoryMock.Verify(r => r.AddPlateAsync(It.IsAny<Plate>()), Times.Once);
        }

        [Fact]
        public async Task GetPlatesAsync_ShouldReturnPagedPlates()
        {
            var plates = new List<Plate> { new Plate("REG1", 100, 150, "A", 1) };
            _plateRepositoryMock.Setup(r => r.GetPlatesAsync(10, 0, true, null)).ReturnsAsync(plates);
            _plateRepositoryMock.Setup(r => r.GetTotalPlatesCountAsync(null)).ReturnsAsync(1);

            var (resultPlates, totalCount) = await _plateService.GetPlatesAsync(10, 0);

            Assert.Single(resultPlates);
            Assert.Equal(1, totalCount);
        }

        [Theory]
        [InlineData(PlateStatus.Available, 2)]
        [InlineData(PlateStatus.Reserved, 1)]
        [InlineData(PlateStatus.Sold, 1)]
        [InlineData(null, 4)]
        public async Task GetPlatesAsync_ShouldReturnPagedPlates_WithDifferentStatuses(PlateStatus? status, int expectedCount)
        {
            // Arrange
            var allPlates = new List<Plate>
            {
                new Plate("REG1", 100, 150, "A", 1) { Status = PlateStatus.Available },
                new Plate("REG2", 100, 150, "A", 1) { Status = PlateStatus.Available },
                new Plate("REG3", 200, 250, "B", 2) { Status = PlateStatus.Reserved },
                new Plate("REG4", 300, 350, "C", 3) { Status = PlateStatus.Sold }
            };

            IEnumerable<Plate> filteredPlates = status switch
            {
                PlateStatus.Available => allPlates.Where(p => p.Status == PlateStatus.Available),
                PlateStatus.Reserved => allPlates.Where(p => p.Status == PlateStatus.Reserved),
                PlateStatus.Sold => allPlates.Where(p => p.Status == PlateStatus.Sold),
                _ => allPlates
            };

            _plateRepositoryMock.Setup(r => r.GetPlatesAsync(10, 0, true, status))
                .ReturnsAsync(filteredPlates);
            _plateRepositoryMock.Setup(r => r.GetTotalPlatesCountAsync(status))
                .ReturnsAsync(filteredPlates.Count());

            // Act
            var (plates, totalCount) = await _plateService.GetPlatesAsync(10, 0, true, status);

            // Assert
            Assert.Equal(expectedCount, plates.Count());
            Assert.Equal(expectedCount, totalCount);
        }

        [Fact]
        public async Task GetPlatesByLettersAsync_ShouldReturnFilteredPlates()
        {
            var letters = "ABC";
            var expected = (new List<Plate> { new Plate("REG1", 100, 150, "ABC", 1) }, 1);
            _plateRepositoryMock.Setup(r => r.GetPlatesByLettersAsync(letters, 10, 0)).ReturnsAsync(expected);

            var result = await _plateService.GetPlatesByLettersAsync(letters, 10, 0);

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task GetPlatesByNumbersAsync_ShouldReturnFilteredPlates()
        {
            var numbers = "123";
            var expected = (new List<Plate> { new Plate("REG2", 200, 250, "XYZ", 123) }, 1);
            _plateRepositoryMock.Setup(r => r.GetPlatesByNumbersAsync(numbers, 10, 0)).ReturnsAsync(expected);

            var result = await _plateService.GetPlatesByNumbersAsync(numbers, 10, 0);

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task SearchPlatesAsync_ShouldReturnSearchResults()
        {
            var query = "REG";
            var expected = (new List<Plate> { new Plate("REGX", 300, 350, "LMN", 456) }, 1);
            _plateRepositoryMock.Setup(r => r.SearchPlatesAsync(query, 10, 0)).ReturnsAsync(expected);

            var result = await _plateService.SearchPlatesAsync(query, 10, 0);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(PlateStatus.Available, PlateStatus.Reserved, true)]
        [InlineData(PlateStatus.Reserved, PlateStatus.Sold, true)]
        [InlineData(PlateStatus.Sold, PlateStatus.Available, true)]
        [InlineData(PlateStatus.Available, PlateStatus.Available, false)]
        [InlineData(PlateStatus.Reserved, PlateStatus.Reserved, false)]
        [InlineData(PlateStatus.Sold, PlateStatus.Sold, false)]
        public async Task UpdatePlateStatusAsync_ShouldHandleStatusTransitionsCorrectly(PlateStatus currentStatus, PlateStatus newStatus, bool shouldUpdate)
        {
            // Arrange
            var plateId = Guid.NewGuid();
            var plate = new Plate("REG", 500, 1000, "XYZ", 123) { Status = currentStatus };

            _plateRepositoryMock.Setup(r => r.GetPlateByIdAsync(plateId)).ReturnsAsync(plate);

            if (shouldUpdate)
            {
                _plateRepositoryMock.Setup(r => r.UpdatePlateAsync(It.IsAny<Plate>())).Returns(Task.CompletedTask);
                _busMock.Setup(b => b.Publish(It.IsAny<PlateStatusChangedEvent>(), default)).Returns(Task.CompletedTask);
            }

            // Act
            var result = await _plateService.UpdatePlateStatusAsync(plateId, newStatus);

            // Assert
            Assert.Equal(newStatus, result.Status);

            if (shouldUpdate)
            {
                _plateRepositoryMock.Verify(r => r.UpdatePlateAsync(It.Is<Plate>(p => p.Status == newStatus)), Times.Once);
                _busMock.Verify(b => b.Publish(It.Is<PlateStatusChangedEvent>(e =>
                    e.PlateId == plateId &&
                    e.OldStatus == currentStatus &&
                    e.NewStatus == newStatus
                ), default), Times.Once);
            }
            else
            {
                _plateRepositoryMock.Verify(r => r.UpdatePlateAsync(It.IsAny<Plate>()), Times.Never);
                _busMock.Verify(b => b.Publish(It.IsAny<PlateStatusChangedEvent>(), default), Times.Never);
            }
        }

        [Fact]
        public async Task UpdatePlateStatusAsync_ShouldThrow_WhenPlateNotFound()
        {
            var plateId = Guid.NewGuid();
            _plateRepositoryMock.Setup(r => r.GetPlateByIdAsync(plateId)).ReturnsAsync((Plate?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _plateService.UpdatePlateStatusAsync(plateId, PlateStatus.Sold));
        }

        [Fact]
        public async Task GetTotalRevenueAsync_ShouldReturnRevenue()
        {
            var expectedRevenue = new Revenue
            {
                AverageProfitMargin = 25,
                TotalSalePrice = 1000
            };
            _plateRepositoryMock.Setup(r => r.GetTotalRevenueAsync()).ReturnsAsync(expectedRevenue);

            var result = await _plateService.GetTotalRevenueAsync();

            Assert.Equal(expectedRevenue, result);
        }
    }
}