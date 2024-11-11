using System.Net.WebSockets;

namespace CodingChallenge.Services
{
    public class WebSocketClientFactory : IWebSocketClientFactory
    {
        public ClientWebSocket CreateClientWebSocket()
        {
            return new ClientWebSocket();
        }
    }
}
