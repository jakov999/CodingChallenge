using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodingChallenge.Models;
using CodingChallenge.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace CodingChallenge.Tests.Services
{
    [TestFixture]
    public class WebSocketServiceTests
    {
        private Mock<IServiceScopeFactory> _mockScopeFactory;
        private Mock<IServiceScope> _mockScope;
        private Mock<ICryptoPriceService> _mockCryptoPriceService;
        private Mock<ILogger<WebSocketService>> _mockLogger;
        private WebSocketService _webSocketService;
        private CancellationTokenSource _cts;

        [SetUp]
        public void SetUp()
        {
            _mockScopeFactory = new Mock<IServiceScopeFactory>();
            _mockScope = new Mock<IServiceScope>();
            _mockCryptoPriceService = new Mock<ICryptoPriceService>();
            _mockLogger = new Mock<ILogger<WebSocketService>>();
            _cts = new CancellationTokenSource();

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(sp => sp.GetService(typeof(ICryptoPriceService)))
                .Returns(_mockCryptoPriceService.Object);

            _mockScope.Setup(s => s.ServiceProvider).Returns(serviceProvider.Object);
            _mockScopeFactory.Setup(sf => sf.CreateScope()).Returns(_mockScope.Object);

            _webSocketService = new WebSocketService(_mockScopeFactory.Object, _mockLogger.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _cts.Dispose();
        }

        [Test]
        public async Task ExecuteAsync_ConnectsToWebSocketAndProcessesMessages()
        {
            // Arrange
            var testPrices = new Dictionary<string, string> { { "bitcoin", "45000.00" }, { "ethereum", "3000.00" } };
            var message = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(testPrices));
            var webSocket = new Mock<ClientWebSocket>();
            webSocket.Setup(ws => ws.State).Returns(WebSocketState.Open);
            webSocket.SetupSequence(ws => ws.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new WebSocketReceiveResult(message.Length, WebSocketMessageType.Text, true));

            // Act
            var executeTask = _webSocketService.StartAsync(_cts.Token);
            await Task.Delay(1000); // Allow some time for execution
            _cts.Cancel();
            await executeTask;

            // Assert
            _mockCryptoPriceService.Verify(service => service.AddCryptoPriceAsync(It.Is<CryptoPrice>(price =>
                price.Currency == "bitcoin" && price.Price == 45000.00m && price.DateReceived == DateTime.UtcNow)), Times.Once);

            _mockCryptoPriceService.Verify(service => service.AddCryptoPriceAsync(It.Is<CryptoPrice>(price =>
                price.Currency == "ethereum" && price.Price == 3000.00m && price.DateReceived == DateTime.UtcNow)), Times.Once);
        }

        [Test]
        public async Task ExecuteAsync_HandlesWebSocketConnectionError()
        {
            // Arrange
            var webSocket = new Mock<ClientWebSocket>();
            webSocket.Setup(ws => ws.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Connection error"));

            // Act
            var executeTask = _webSocketService.StartAsync(_cts.Token);
            await Task.Delay(1000); // Allow some time for execution
            _cts.Cancel();
            await executeTask;

            // Assert
            _mockLogger.Verify(
                x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task ExecuteAsync_RetriesOnConnectionError()
        {
            // Arrange
            int maxRetries = 3;
            int retryCount = 0;

            var webSocket = new Mock<ClientWebSocket>();
            webSocket.Setup(ws => ws.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .Callback(() => retryCount++)
                .ThrowsAsync(new Exception("Connection error"));

            // Act
            var executeTask = _webSocketService.StartAsync(_cts.Token);
            await Task.Delay(5000); // Allow some time for retries
            _cts.Cancel();
            await executeTask;

            // Assert
            ClassicAssert.LessOrEqual(retryCount, maxRetries);
            _mockLogger.Verify(x => x.LogInformation(It.IsAny<string>()), Times.AtLeastOnce);
        }
    }
}
