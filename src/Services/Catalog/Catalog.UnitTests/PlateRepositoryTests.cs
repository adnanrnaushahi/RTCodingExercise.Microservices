using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.API.Data;
using Catalog.API.Repositories;
using Catalog.API.Services;
using Catalog.Domain.Entities;
using Catalog.Domain.Enum;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Models;
using IntegrationEvents.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Catalog.UnitTests
{
    public class PlateRepositoryTests
    {
        private readonly PlateRepository _plateRepository;
        private readonly ApplicationDbContext _dbContext;

        public PlateRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _plateRepository = new PlateRepository(_dbContext);
        }

        [Fact]
        public async Task GetTotalRevenueAsync_ShouldCalculateCorrectly()
        {
            // Arrange
            _dbContext.Plates.Add(new Plate("ABC1", 100, 120, "AB", 1) { Status = PlateStatus.Sold });
            _dbContext.Plates.Add(new Plate("ABC2", 100, 150, "AB", 2) { Status = PlateStatus.Sold });
            _dbContext.Plates.Add(new Plate("CDE3", 200, 280, "CD", 3) { Status = PlateStatus.Sold });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _plateRepository.GetTotalRevenueAsync();

            // Assert
            Assert.Equal(550, result.TotalSalePrice); // 120 + 240
            Assert.True(result.AverageProfitMargin > 0);
            Assert.Equal(13.89M, result.AverageProfitMargin);
        }
    }
}