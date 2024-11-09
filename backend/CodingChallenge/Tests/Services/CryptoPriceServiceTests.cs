using CodingChallenge.Models;
using CodingChallenge.Services;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using CodingChallenge.Data.Repositories;
namespace CodingChallenge.Tests.Services
{
    [TestFixture]
    public class CryptoPriceServiceTests
    {

        private Mock<ICryptoPriceRepository> _cryptoPriceRepositoryMock;
        private CryptoService _cryptoPriceService;

        [SetUp]
        public void Setup()
        {
            
            _cryptoPriceRepositoryMock = new Mock<ICryptoPriceRepository>();

            // Set up UnitOfWork to return the mock repository
           

            //_cryptoPriceService = new CryptoService();
        }

        [Test]
        public async Task GetAllPricesAsync_ShouldReturnAllPrices()
        {
            // Arrange
            var mockPrices = new List<CryptoPrice>
        {
            new CryptoPrice { Id = 1, Currency = "bitcoin", Price = 50000, DateReceived = DateTime.UtcNow },
            new CryptoPrice { Id = 2, Currency = "ethereum", Price = 4000, DateReceived = DateTime.UtcNow }
        };

            _cryptoPriceRepositoryMock.Setup(repo => repo.GetAllPricesAsync()).ReturnsAsync(mockPrices);

            // Act
            var result = await _cryptoPriceService.GetAllPricesAsync();

            // Assert
            result.Should().BeEquivalentTo(mockPrices);
            _cryptoPriceRepositoryMock.Verify(repo => repo.GetAllPricesAsync(), Times.Once);
        }

        [Test]
        public async Task GetLatestPricesAsync_ShouldReturnLatestPriceForEachCurrency()
        {
            // Arrange
            var mockPrices = new List<CryptoPrice>
        {
            new CryptoPrice { Id = 1, Currency = "bitcoin", Price = 50000, DateReceived = DateTime.UtcNow.AddMinutes(-10) },
            new CryptoPrice { Id = 2, Currency = "bitcoin", Price = 51000, DateReceived = DateTime.UtcNow },
            new CryptoPrice { Id = 3, Currency = "ethereum", Price = 4000, DateReceived = DateTime.UtcNow.AddMinutes(-5) }
        };

            var expectedPrices = new List<CryptoPrice>
        {
            mockPrices[1], // Latest price for Bitcoin
            mockPrices[2]  // Latest price for Ethereum
        };

            _cryptoPriceRepositoryMock.Setup(repo => repo.GetLatestPricesAsync()).ReturnsAsync(expectedPrices);

            // Act
            var result = await _cryptoPriceService.GetLatestPricesAsync();

            // Assert
            result.Should().BeEquivalentTo(expectedPrices);
            _cryptoPriceRepositoryMock.Verify(repo => repo.GetLatestPricesAsync(), Times.Once);
        }

        [Test]
        public async Task GetPricesByCurrencyAsync_WithValidCurrency_ShouldReturnPrices()
        {
            // Arrange
            var currency = "bitcoin";
            var mockPrices = new List<CryptoPrice>
        {
            new CryptoPrice { Id = 1, Currency = currency, Price = 50000, DateReceived = DateTime.UtcNow.AddMinutes(-10) },
            new CryptoPrice { Id = 2, Currency = currency, Price = 51000, DateReceived = DateTime.UtcNow }
        };

            _cryptoPriceRepositoryMock.Setup(repo => repo.GetPricesByCurrencyAsync(currency)).ReturnsAsync(mockPrices);

            // Act
            var result = await _cryptoPriceService.GetPricesByCurrencyAsync(currency);

            // Assert
            result.Should().BeEquivalentTo(mockPrices);
            _cryptoPriceRepositoryMock.Verify(repo => repo.GetPricesByCurrencyAsync(currency), Times.Once);
        }

        [Test]
        public async Task GetPricesByCurrencyAsync_WithInvalidCurrency_ShouldReturnEmptyList()
        {
            // Arrange
            var currency = "nonexistent";
            _cryptoPriceRepositoryMock.Setup(repo => repo.GetPricesByCurrencyAsync(currency)).ReturnsAsync(new List<CryptoPrice>());

            // Act
            var result = await _cryptoPriceService.GetPricesByCurrencyAsync(currency);

            // Assert
            result.Should().BeEmpty();
            _cryptoPriceRepositoryMock.Verify(repo => repo.GetPricesByCurrencyAsync(currency), Times.Once);
        }
    }
}
