using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using WebMVC.Services;
using WebMVC.ViewModels;
using Xunit;
using System.Text.Json;
using Catalog.Domain.Enum;
using FluentAssertions;
using System.Text;

namespace WebMVC.UnitTests
{
    public class PlateServiceTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly Mock<IOptions<AppSettings>> _mockOptions;
        private readonly string _catalogApiUrl = "http://test-catalog-api/";
        private readonly IPlateService _plateService;

        public PlateServiceTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(_catalogApiUrl)
            };

            _mockOptions = new Mock<IOptions<AppSettings>>();
            _mockOptions.Setup(x => x.Value).Returns(new AppSettings { CatalogApiUrl = _catalogApiUrl });

            _plateService = new PlateService(_httpClient, _mockOptions.Object);
        }

        [Fact]
        public async Task GetAllPlatesAsync_ShouldReturnPaginatedPlates()
        {
            // Arrange
            var expected = new PaginatedItemsViewModel<PlateViewModel>
            {
                PageIndex = 0,
                PageSize = 1,
                TotalItems = 1,
                TotalPages = 1,
                Data = new List<PlateViewModel>
            {
                new PlateViewModel
                {
                    Id = Guid.NewGuid(),
                    Registration = "ABC123",
                    PurchasePrice = 100,
                    SalePrice = 150,
                    Letters = "ABC",
                    Numbers = 123,
                    Status = PlateStatus.Available
                }
            }
            };

            SetupMockResponse("api/plates", HttpMethod.Get, HttpStatusCode.OK, expected);

            // Act
            var result = await _plateService.GetAllPlatesAsync(1, 0);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Data);
            Assert.Equal("ABC123", result.Data.First().Registration);
        }

        [Fact]
        public async Task GetPlateByIdAsync_ShouldReturnPlate()
        {
            var id = Guid.NewGuid();
            var expected = new PlateViewModel {
                Id = Guid.NewGuid(),
                Registration = "ABC123",
                PurchasePrice = 100,
                SalePrice = 150,
                Letters = "ABC",
                Numbers = 123,
                Status = PlateStatus.Available
            };

            SetupMockResponse($"api/plates/{id}",HttpMethod.Get, HttpStatusCode.OK, expected);

            var result = await _plateService.GetPlateByIdAsync(id);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task CreatePlateAsync_ShouldReturnCreatedPlate()
        {
            var model = new CreatePlateViewModel { Registration = "XYZ999", PurchasePrice = 20, SalePrice = 50 };
            var expected = new PlateViewModel { Registration = "XYZ999", PurchasePrice = 25, SalePrice = 50 };

            SetupMockResponse("api/plates", HttpMethod.Post, HttpStatusCode.OK, expected);

            var result = await _plateService.CreatePlateAsync(model);

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task FilterByLettersAsync_ShouldReturnFilteredResults()
        {
            var expected = new PaginatedItemsViewModel<PlateViewModel>
            {
                PageIndex = 1,
                PageSize = 10,
                TotalItems = 1,
                Data = new List<PlateViewModel> { new PlateViewModel { Id = Guid.NewGuid(),
                    Registration = "ABC123",
                    PurchasePrice = 100,
                    SalePrice = 150,
                    Letters = "ABC",
                    Numbers = 123,
                    Status = PlateStatus.Available } }
            };

            SetupMockResponse("api/plates/filterByLetters?letters=ABC&pageSize=10&pageIndex=1", HttpMethod.Get, HttpStatusCode.OK, expected);

            var result = await _plateService.FilterByLettersAsync("ABC", 10, 1);

            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(PlateStatus.Sold)]
        [InlineData(PlateStatus.Reserved)]
        [InlineData(PlateStatus.Available)]
        public async Task UpdatePlateStatusAsync_ShouldReturnUpdatedPlate_WhenSuccess(PlateStatus newStatus)
        {
            // Arrange
            var plateId = Guid.NewGuid();           
            var expectedPlate = new PlateViewModel
            {
                Id = plateId,
                Registration = "XYZ999",
                PurchasePrice = 100,
                SalePrice = 200,
                Letters = "XYZ",
                Numbers = 999,
                Status = newStatus
            };

            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(expectedPlate), Encoding.UTF8, "application/json")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Put &&
                        req.RequestUri.ToString().EndsWith($"/api/plates/{plateId}/status")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _plateService.UpdatePlateStatusAsync(plateId, newStatus);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedPlate.Id, result.Id);
            Assert.Equal(expectedPlate.Status, result.Status);
            Assert.Equal(expectedPlate.Registration, result.Registration);
        }

        [Fact]
        public async Task UpdatePlateStatusAsync_ShouldThrowException_WhenRequestFails()
        {
            // Arrange
            var plateId = Guid.NewGuid();
            var newStatus = PlateStatus.Sold;

            var errorMessage = "Something went wrong";
            var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(errorMessage)
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Put &&
                        req.RequestUri.ToString().EndsWith($"/api/plates/{plateId}/status")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(() =>
                _plateService.UpdatePlateStatusAsync(plateId, newStatus));

            Assert.Contains("Error update plate status", ex.Message);
            Assert.Contains("BadRequest", ex.Message);
        }

        [Fact]
        public async Task GetTotalRevenueAsync_ShouldReturnRevenue_WhenSuccessful()
        {
            // Arrange
            var expectedRevenue = new RevenueViewModel
            {
                TotalSalePrice = 10000m,
                AverageProfitMargin = 30.0m
            };

            // Use existing SetupMockResponse (assumes "api/plates/GetRevenue" is the key)
            SetupMockResponse("api/plates/GetRevenue",HttpMethod.Get, HttpStatusCode.OK, expectedRevenue);

            // Act
            var result = await _plateService.GetTotalRevenueAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedRevenue.TotalSalePrice, result.TotalSalePrice);
            Assert.Equal(expectedRevenue.AverageProfitMargin, result.AverageProfitMargin);
        }

        // Helper to setup mocked response
        private void SetupMockResponse(string relativePathContains, HttpMethod method, HttpStatusCode status, object responseObj = null)
        {
            var json = JsonSerializer.Serialize(responseObj ?? new object());

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == method &&
                        req.RequestUri != null &&
                        req.RequestUri.ToString().Contains(relativePathContains)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = status,
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                });
        }
    }
}