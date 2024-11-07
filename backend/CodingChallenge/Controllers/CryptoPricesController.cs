using CodingChallenge.Data;
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
    private readonly AppDbContext _context;

    public CryptoPricesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/v1/cryptoprices
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CryptoPrice>>> GetAllPrices()
    {
        return await _context.CryptoPrices.ToListAsync();
    }

    // GET: api/v1/cryptoprices/latest
    [HttpGet("latest")]
    public async Task<ActionResult<IEnumerable<CryptoPrice>>> GetLatestPrices()
    {
        var latestPrices = await _context.CryptoPrices
            .GroupBy(p => p.Currency)
            .Select(g => g.OrderByDescending(p => p.DateReceived).First())
            .ToListAsync();

        return Ok(latestPrices);
    }

    // GET: api/v1/cryptoprices/{currency}

    [HttpGet("{currency}")]
    public async Task<ActionResult<IEnumerable<CryptoPrice>>> GetPricesByCurrency(string currency)
    {
        var prices = await _context.CryptoPrices
            .Where(c => c.Currency.ToLower() == currency.ToLower())
            .OrderByDescending(p => p.DateReceived)
            .Take(10)
            .ToListAsync();

        if (!prices.Any())
            return NotFound();

        return Ok(prices);
    }
}
