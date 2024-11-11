using CodingChallenge.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Net.WebSockets;
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
            // Arrange
            var service = new WebSocketService(_scopeFactoryMock.Object, _loggerMock.Object, _clientFactoryMock.Object);

            // Act
            await service.StartAsync(CancellationToken.None);

            // Assert
            _clientWebSocketMock.Verify(ws => ws.ConnectAsync(It.IsAny<Uri>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
