using CodingChallenge.Models;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace CodingChallenge.Services
{
    public class WebSocketService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<WebSocketService> _logger;

        public WebSocketService(IServiceScopeFactory scopeFactory, ILogger<WebSocketService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            int retryCount = 0;
            const int maxRetries = 10;

            while (!stoppingToken.IsCancellationRequested)
            {
                using var client = new ClientWebSocket();

                try
                {
                    await client.ConnectAsync(new Uri("wss://ws.coincap.io/prices?assets=bitcoin,ethereum,monero"), stoppingToken);
                    retryCount = 0; // Reset retry count on successful connection

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
                                var cryptoService = scope.ServiceProvider.GetRequiredService<ICryptoPriceService>();

                                foreach (var item in priceData)
                                {
                                    var cryptoPrice = new CryptoPrice
                                    {
                                        Currency = item.Key,
                                        Price = decimal.Parse(item.Value),
                                        DateReceived = dateReceived
                                    };

                                    await cryptoService.AddCryptoPriceAsync(cryptoPrice);
                                }
                            }
                        }
                        else if (result.MessageType == WebSocketMessageType.Close)
                        {
                            _logger.LogInformation("WebSocket connection closed.");
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"WebSocket connection error: {ex.Message}");

                    if (++retryCount >= maxRetries)
                    {
                        _logger.LogError("Max retry limit reached. Exiting WebSocket service.");
                        break;
                    }

                    int delaySeconds = retryCount * 10; // Exponential backoff
                    _logger.LogInformation($"Retrying in {delaySeconds} seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds), stoppingToken);
                }
            }
        }
    }
}
