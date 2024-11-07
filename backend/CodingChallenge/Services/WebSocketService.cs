using CodingChallenge.Data;
using CodingChallenge.Models;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace CodingChallenge.Services
{
    public class WebSocketService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public WebSocketService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var client = new ClientWebSocket();
            await client.ConnectAsync(new Uri("wss://ws.coincap.io/prices?assets=bitcoin,ethereum,monero"), stoppingToken);

            var buffer = new byte[1024 * 4];

            while (client.State == WebSocketState.Open && !stoppingToken.IsCancellationRequested)
            {
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var priceData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                    if (priceData != null)
                    {
                        var dateReceived = DateTime.UtcNow;

                        using var scope = _scopeFactory.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        foreach (var item in priceData)
                        {
                            var cryptoPrice = new CryptoPrice
                            {
                                Currency = item.Key,
                                Price = decimal.Parse(item.Value),
                                DateReceived = dateReceived
                            };

                            dbContext.CryptoPrices.Add(cryptoPrice);
                        }

                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine("WebSocket connection closed.");
                    break;
                }
            }
        }
    }
}
