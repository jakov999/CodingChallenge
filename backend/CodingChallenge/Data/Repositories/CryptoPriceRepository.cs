using CodingChallenge.Models;
using Microsoft.EntityFrameworkCore;

namespace CodingChallenge.Data.Repositories
{
    public class CryptoPriceRepository : ICryptoPriceRepository
    {
        private readonly AppDbContext _context;
        public CryptoPriceRepository(AppDbContext context) {
            _context = context;

        }

        public async Task<IEnumerable<CryptoPrice>> GetAllPricesAsync()
        {
            return await _context.CryptoPrices.ToListAsync();
        }
        public async Task<IEnumerable<CryptoPrice>> GetLatestPricesAsync()
        {
            return await _context.CryptoPrices
                .GroupBy(p => p.Currency)
                .Select(g => g.OrderByDescending(p => p.DateReceived).First())
                .ToListAsync();
        }

        public async Task<IEnumerable<CryptoPrice>> GetPricesByCurrencyAsync(string currency)
        {
            return await _context.CryptoPrices
                .Where(c => c.Currency.ToLower() == currency.ToLower())
                .OrderByDescending(p => p.DateReceived)
                .Take(10)
                .ToListAsync();
        }
        public async Task AddCryptoPriceAsync(CryptoPrice cryptoPrice)
        {
            await _context.CryptoPrices.AddAsync(cryptoPrice);
            await _context.SaveChangesAsync();
        }
    }
}
