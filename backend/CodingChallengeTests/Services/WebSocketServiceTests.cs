using System;
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

        //[Test]
        //public async Task WebSocketService_AttemptsToConnect_OnActivation()
        //{
        //    // Arrange
        //    var clientWebSocket = new ClientWebSocket();
        //    clientWebSocket.Setup(ws => ws.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
        //        .Returns(Task.CompletedTask);

        //    // Act
        //    var executeTask = _webSocketService.StartAsync(_cts.Token);
        //    await Task.Delay(1000); // Allow some time for connection attempt
        //    _cts.Cancel();
        //    await executeTask;

        //    // Assert
        //    _mockLogger.Verify(log => log.LogInformation(It.Is<string>(msg => msg.Contains("WebSocket connection closed."))), Times.Never);
        //    _mockLogger.Verify(log => log.LogError(It.IsAny<string>()), Times.Never);
        //}

        [Test]
        public async Task WebSocketService_RetriesConnection_OnFailure()
        {
            // Arrange
            int retryCount = 0;
            const int maxRetries = 3;
            var clientWebSocket = new Mock<ClientWebSocket>();

            clientWebSocket.Setup(ws => ws.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .Callback(() => retryCount++)
                .ThrowsAsync(new Exception("Connection failed"));

            // Act
            var executeTask = _webSocketService.StartAsync(_cts.Token);
            await Task.Delay(5000); // Allow some time for retries
            _cts.Cancel();
            await executeTask;

            // Assert
            ClassicAssert.LessOrEqual(retryCount, maxRetries);
            _mockLogger.Verify(log => log.LogError(It.Is<string>(msg => msg.Contains("Connection failed"))), Times.AtLeastOnce);
            _mockLogger.Verify(log => log.LogInformation(It.Is<string>(msg => msg.Contains("Retrying"))), Times.AtLeastOnce);
        }

        [Test]
        public async Task WebSocketService_StopsRetryingAfterMaxLimit()
        {
            // Arrange
            int retryCount = 0;
            const int maxRetries = 10;
            var clientWebSocket = new Mock<ClientWebSocket>();

            clientWebSocket.Setup(ws => ws.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
                .Callback(() => retryCount++)
                .ThrowsAsync(new Exception("Connection failed"));

            // Act
            var executeTask = _webSocketService.StartAsync(_cts.Token);
            await Task.Delay(11000); // Allow enough time to reach max retries
            _cts.Cancel();
            await executeTask;

            // Assert
            ClassicAssert.AreEqual(maxRetries, retryCount);
            _mockLogger.Verify(log => log.LogError("Max retry limit reached. Exiting WebSocket service."), Times.Once);
        }
    }
}
