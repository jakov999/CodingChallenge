using CodingChallenge.Models;
using CodingChallenge.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Net.WebSockets;
using System.Text;
using Xunit;
namespace CodingChallenge.Tests.Services
{
    [TestFixture]
    public class WebSocketServiceTests
    {
        private Mock<IServiceScopeFactory> _scopeFactoryMock;
        private Mock<ILogger<WebSocketService>> _loggerMock;
        private Mock<IWebSocketClientFactory> _clientFactoryMock;
        private Mock<ClientWebSocket> _clientWebSocketMock;

        [SetUp]
        public void SetUp()
        {
            _scopeFactoryMock = new Mock<IServiceScopeFactory>();
            _loggerMock = new Mock<ILogger<WebSocketService>>();
            _clientFactoryMock = new Mock<IWebSocketClientFactory>();
            _clientWebSocketMock = new Mock<ClientWebSocket>();

            _clientFactoryMock.Setup(factory => factory.CreateClientWebSocket()).Returns(_clientWebSocketMock.Object);
        }

        [Test]
        public async Task WebSocketService_ConnectsToWebSocket_Successfully()
        {
            
            var service = new WebSocketService(_scopeFactoryMock.Object, _loggerMock.Object, _clientFactoryMock.Object);

            
            await service.StartAsync(CancellationToken.None);

            
            _clientWebSocketMock.Verify(ws => ws.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact]
        public async Task WebSocketService_Should_ProcessMessages()
        {
            // Arrange
            var mockFactory = new Mock<IWebSocketClientFactory>();
            var mockClient = new Mock<ClientWebSocket>();

            // Simulate WebSocket messages
            mockClient.SetupSequence(c => c.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new WebSocketReceiveResult(Encoding.UTF8.GetBytes("{\"bitcoin\":\"30000\"}").Length, WebSocketMessageType.Text, true))
                .ReturnsAsync(new WebSocketReceiveResult(0, WebSocketMessageType.Close, true));

            // Configure factory to return our mock client
            mockFactory.Setup(f => f.CreateClientWebSocket()).Returns(mockClient.Object);

            // Configure other dependencies if necessary
            var mockCryptoService = new Mock<ICryptoPriceService>();

            var logger = Mock.Of<ILogger<WebSocketService>>();
            var scopeFactory = Mock.Of<IServiceScopeFactory>();

            // Instantiate the service with mocks
            var service = new WebSocketService(scopeFactory, logger, mockFactory.Object);

            // Act
            await service.StartAsync(CancellationToken.None);

            // Assert
            mockCryptoService.Verify(c => c.AddCryptoPriceAsync(It.IsAny<CryptoPrice>()), Times.Once);
        }
    }
}
