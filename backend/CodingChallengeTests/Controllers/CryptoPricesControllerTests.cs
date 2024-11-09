using CodingChallenge.Models;
using CodingChallenge.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodingChallenge.Tests.Controllers
{
    [TestFixture]
    public class CryptoPricesControllerTests
    {
        private Mock<ICryptoPriceService> _mockCryptoPriceService;
        private CryptoPricesController _controller;

        [SetUp]
        public void Setup()
        {
            _mockCryptoPriceService = new Mock<ICryptoPriceService>();
            _controller = new CryptoPricesController(_mockCryptoPriceService.Object);
        }

        [Test]
        public async Task GetAllPrices_ReturnsOkResult_WithListOfCryptoPrices()
        {
            // Arrange
            var mockPrices = new List<CryptoPrice>
            {
                new CryptoPrice { Currency = "BTC", Price = 50000.0m },
                new CryptoPrice { Currency = "ETH", Price = 3000.0m }
            };
            _mockCryptoPriceService.Setup(service => service.GetAllPricesAsync()).ReturnsAsync(mockPrices);

            // Act
            var result = await _controller.GetAllPrices();

            // Assert
            var okResult = result.Result as OkObjectResult;
            ClassicAssert.IsNotNull(okResult);
            var returnValue = okResult.Value as IEnumerable<CryptoPrice>;
            ClassicAssert.IsNotNull(returnValue);
            ClassicAssert.AreEqual(2, returnValue.Count());
        }

        [Test]
        public async Task GetLatestPrices_ReturnsOkResult_WithLatestCryptoPrices()
        {
            // Arrange
            var mockLatestPrices = new List<CryptoPrice>
            {
                new CryptoPrice { Currency = "BTC", Price = 52000.0m },
                new CryptoPrice { Currency = "ETH", Price = 3100.0m }
            };
            _mockCryptoPriceService.Setup(service => service.GetLatestPricesAsync()).ReturnsAsync(mockLatestPrices);

            // Act
            var result = await _controller.GetLatestPrices();

            // Assert
            var okResult = result.Result as OkObjectResult;
            ClassicAssert.IsNotNull(okResult);
            var returnValue = okResult.Value as IEnumerable<CryptoPrice>;
            ClassicAssert.IsNotNull(returnValue);
            ClassicAssert.AreEqual(2, returnValue.Count());
        }

        [Test]
        public async Task GetPricesByCurrency_ReturnsOkResult_WithPricesForGivenCurrency()
        {
            // Arrange
            var mockPrices = new List<CryptoPrice>
            {
                new CryptoPrice { Currency = "BTC", Price = 52000.0m },
                new CryptoPrice { Currency = "BTC", Price = 51000.0m }
            };
            _mockCryptoPriceService.Setup(service => service.GetPricesByCurrencyAsync("BTC")).ReturnsAsync(mockPrices);

            // Act
            var result = await _controller.GetPricesByCurrency("BTC");

            // Assert
            var okResult = result.Result as OkObjectResult;
            ClassicAssert.IsNotNull(okResult);
            var returnValue = okResult.Value as IEnumerable<CryptoPrice>;
            ClassicAssert.IsNotNull(returnValue);
            ClassicAssert.AreEqual(2, returnValue.Count());
        }

        [Test]
        public async Task GetPricesByCurrency_ReturnsNotFound_WhenNoPricesForGivenCurrency()
        {
            // Arrange
            _mockCryptoPriceService.Setup(service => service.GetPricesByCurrencyAsync("Unknown")).ReturnsAsync((IEnumerable<CryptoPrice>)null);

            // Act
            var result = await _controller.GetPricesByCurrency("Unknown");

            // Assert
            ClassicAssert.IsInstanceOf<NotFoundResult>(result.Result);
        }
    }
}
