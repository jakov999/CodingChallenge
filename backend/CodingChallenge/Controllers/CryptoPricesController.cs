using CodingChallenge.Data;
using CodingChallenge.Models;
using CodingChallenge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/v1/cryptoprices")]
public class CryptoPricesController : ControllerBase
{
    private readonly ICryptoPriceService _cryptoPriceService;
    private readonly ILogger<CryptoPricesController> _logger;
    public CryptoPricesController(ICryptoPriceService cryptoPriceService, ILogger<CryptoPricesController> logger)
    {
        _cryptoPriceService = cryptoPriceService;
        _logger = logger;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<CryptoPrice>>> GetAllPrices()
    {
        var prices = await _cryptoPriceService.GetAllPricesAsync();
        _logger.LogInformation("CryptoPricesController GetAllPricesExecutedSuccessfully");
        return Ok(prices);
    }


    [HttpGet("latest")]
    public async Task<ActionResult<IEnumerable<CryptoPrice>>> GetLatestPrices()
    {
        var latestPrices = await _cryptoPriceService.GetLatestPricesAsync();
        _logger.LogInformation("CryptoPricesController GetLatestPrices ExecutedSuccessfully");
        return Ok(latestPrices);
    }


    [HttpGet("{currency}")]
    public async Task<ActionResult<IEnumerable<CryptoPrice>>> GetPricesByCurrency(string currency)
    {
        var prices = await _cryptoPriceService.GetPricesByCurrencyAsync(currency);
        if (prices == null || !prices.Any())
        {
            _logger.LogWarning($"CryptoPricesController, {currency} has no prices available");
            return NotFound();
        }
        return Ok(prices);
    }
}
