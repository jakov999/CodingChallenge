using CodingChallenge.Data;
using CodingChallenge.Interfaces;
using CodingChallenge.Models;
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

    public CryptoPricesController(ICryptoPriceService cryptoPriceService)
    {
        _cryptoPriceService = cryptoPriceService;
    }

    // GET: api/v1/cryptoprices
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CryptoPrice>>> GetAllPrices()
    {
        var prices = await _cryptoPriceService.GetAllPricesAsync();
        return Ok(prices);
    }

    // GET: api/v1/cryptoprices/latest
    [HttpGet("latest")]
    public async Task<ActionResult<IEnumerable<CryptoPrice>>> GetLatestPrices()
    {
        var latestPrices = await _cryptoPriceService.GetLatestPricesAsync();
        return Ok(latestPrices);
    }

    // GET: api/v1/cryptoprices/{currency}
    [HttpGet("{currency}")]
    public async Task<ActionResult<IEnumerable<CryptoPrice>>> GetPricesByCurrency(string currency)
    {
        var prices = await _cryptoPriceService.GetPricesByCurrencyAsync(currency);
        if (prices == null || !prices.Any())
            return NotFound();

        return Ok(prices);
    }
}
