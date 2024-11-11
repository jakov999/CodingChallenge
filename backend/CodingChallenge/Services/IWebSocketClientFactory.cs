using System.Net.WebSockets;

namespace CodingChallenge.Services
{
    public interface IWebSocketClientFactory
    {
        ClientWebSocket CreateClientWebSocket();
    }

}
