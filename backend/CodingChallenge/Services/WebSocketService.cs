﻿using CodingChallenge.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CodingChallenge.Services
{
    public class WebSocketService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<WebSocketService> _logger;
        private readonly IWebSocketClientFactory _clientFactory;
        public WebSocketService(IServiceScopeFactory scopeFactory, ILogger<WebSocketService> logger, IWebSocketClientFactory clientFactory)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _clientFactory = clientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            int retryCount = 0;
            const int maxRetries = 10;
            using var scope = _scopeFactory.CreateScope();
            var cryptoService = scope.ServiceProvider.GetRequiredService<ICryptoPriceService>();

            while (!stoppingToken.IsCancellationRequested)
            {
                using var client = _clientFactory.CreateClientWebSocket();

                try
                {
                    await client.ConnectAsync(new Uri("wss://ws.coincap.io/prices?assets=bitcoin,ethereum,monero"), stoppingToken);
                    retryCount = 0;

                    var buffer = new byte[1024];

                    while (client.State == WebSocketState.Open && !stoppingToken.IsCancellationRequested)
                    {
                        using var memoryStream = new MemoryStream();

                        WebSocketReceiveResult result;
                        do
                        {
                            result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);
                            memoryStream.Write(buffer, 0, result.Count);
                        }
                        while (!result.EndOfMessage && !stoppingToken.IsCancellationRequested);

                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            var json = Encoding.UTF8.GetString(memoryStream.ToArray());
                            var priceData = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                            if (priceData != null)
                            {
                                var dateReceived = DateTime.UtcNow;

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
                            await client.CloseAsync(WebSocketCloseStatus.NormalClosure,
                                                    "Connection closed by client",
                                                    CancellationToken.None);
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

                    double delaySeconds = Math.Pow(2, retryCount);
                    _logger.LogInformation($"Retrying in {delaySeconds} seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds), stoppingToken);
                }
            }
        }
        
    }
}
