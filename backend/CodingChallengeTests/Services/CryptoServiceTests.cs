using CodingChallenge.Data.Repositories;
using CodingChallenge.Models;
using CodingChallenge.Services;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingChallenge.Tests.Services
{
    [TestFixture]
    public class CryptoServiceTests
    {
        private Mock<ICryptoPriceRepository> _mockCryptoPriceRepository;
        private CryptoService _cryptoService;

        [SetUp]
        public void Setup()
        {
            _mockCryptoPriceRepository = new Mock<ICryptoPriceRepository>();
            _cryptoService = new CryptoService(_mockCryptoPriceRepository.Object);
        }

        [Test]
        public async Task GetAllPricesAsync_ReturnsListOfCryptoPrices()
        {
            // Arrange
            var mockPrices = new List<CryptoPrice>
            {
                new CryptoPrice { Currency = "BTC", Price = 50000.0m },
                new CryptoPrice { Currency = "ETH", Price = 3000.0m }
            };
            _mockCryptoPriceRepository.Setup(repo => repo.GetAllPricesAsync()).ReturnsAsync(mockPrices);

            // Act
            var result = await _cryptoService.GetAllPricesAsync();

            // Assert
            ClassicAssert.NotNull(result);
            ClassicAssert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetLatestPricesAsync_ReturnsLatestCryptoPrices()
        {
            // Arrange
            var mockLatestPrices = new List<CryptoPrice>
            {
                new CryptoPrice { Currency = "BTC", Price = 52000.0m },
                new CryptoPrice { Currency = "ETH", Price = 3100.0m }
            };
            _mockCryptoPriceRepository.Setup(repo => repo.GetLatestPricesAsync()).ReturnsAsync(mockLatestPrices);

            // Act
            var result = await _cryptoService.GetLatestPricesAsync();

            // Assert
            ClassicAssert.NotNull(result);
            ClassicAssert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetPricesByCurrencyAsync_ReturnsPricesForSpecifiedCurrency()
        {
            // Arrange
            var mockPrices = new List<CryptoPrice>
            {
                new CryptoPrice { Currency = "BTC", Price = 52000.0m },
                new CryptoPrice { Currency = "BTC", Price = 51000.0m }
            };
            _mockCryptoPriceRepository.Setup(repo => repo.GetPricesByCurrencyAsync("BTC")).ReturnsAsync(mockPrices);

            // Act
            var result = await _cryptoService.GetPricesByCurrencyAsync("BTC");

            // Assert
            ClassicAssert.NotNull(result);
            ClassicAssert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task AddCryptoPriceAsync_CallsRepositoryAddMethod()
        {
            // Arrange
            var newPrice = new CryptoPrice { Currency = "BTC", Price = 55000.0m };

            // Act
            await _cryptoService.AddCryptoPriceAsync(newPrice);

            // Assert
            _mockCryptoPriceRepository.Verify(repo => repo.AddCryptoPriceAsync(newPrice), Times.Once);
        }
    }
}
