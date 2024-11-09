using CodingChallenge.Models;

namespace CodingChallenge.Services
{
    public interface ICryptoPriceService
    {
        Task<IEnumerable<CryptoPrice>> GetAllPricesAsync();
        Task<IEnumerable<CryptoPrice>> GetLatestPricesAsync();
        Task<IEnumerable<CryptoPrice>> GetPricesByCurrencyAsync(string currency);
        Task AddCryptoPriceAsync(CryptoPrice cryptoPrice);
    }
}
