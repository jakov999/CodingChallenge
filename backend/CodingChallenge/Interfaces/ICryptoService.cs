using CodingChallenge.Models;

namespace CodingChallenge.Interfaces
{
    public interface ICryptoPriceService
    {
        Task<IEnumerable<CryptoPrice>> GetAllPricesAsync();
        Task<IEnumerable<CryptoPrice>> GetLatestPricesAsync();
        Task<IEnumerable<CryptoPrice>> GetPricesByCurrencyAsync(string currency);
    }
}
