﻿using CodingChallenge.Data.Repositories;
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
            
            var mockPrices = new List<CryptoPrice>
            {
                new CryptoPrice { Currency = "BTC", Price = 50000.0m },
                new CryptoPrice { Currency = "ETH", Price = 3000.0m }
            };
            _mockCryptoPriceRepository.Setup(repo => repo.GetAllPricesAsync()).ReturnsAsync(mockPrices);

            
            var result = await _cryptoService.GetAllPricesAsync();

            
            ClassicAssert.NotNull(result);
            ClassicAssert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetLatestPricesAsync_ReturnsLatestCryptoPrices()
        {
            
            var mockLatestPrices = new List<CryptoPrice>
            {
                new CryptoPrice { Currency = "BTC", Price = 52000.0m },
                new CryptoPrice { Currency = "ETH", Price = 3100.0m }
            };
            _mockCryptoPriceRepository.Setup(repo => repo.GetLatestPricesAsync()).ReturnsAsync(mockLatestPrices);

            
            var result = await _cryptoService.GetLatestPricesAsync();

            
            ClassicAssert.NotNull(result);
            ClassicAssert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetPricesByCurrencyAsync_ReturnsPricesForSpecifiedCurrency()
        {
            
            var mockPrices = new List<CryptoPrice>
            {
                new CryptoPrice { Currency = "BTC", Price = 52000.0m },
                new CryptoPrice { Currency = "BTC", Price = 51000.0m }
            };
            _mockCryptoPriceRepository.Setup(repo => repo.GetPricesByCurrencyAsync("BTC")).ReturnsAsync(mockPrices);

            
            var result = await _cryptoService.GetPricesByCurrencyAsync("BTC");

            
            ClassicAssert.NotNull(result);
            ClassicAssert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task AddCryptoPriceAsync_CallsRepositoryAddMethod()
        {
            
            var newPrice = new CryptoPrice { Currency = "BTC", Price = 55000.0m };

            
            await _cryptoService.AddCryptoPriceAsync(newPrice);

            
            _mockCryptoPriceRepository.Verify(repo => repo.AddCryptoPriceAsync(newPrice), Times.Once);
        }
    }
}