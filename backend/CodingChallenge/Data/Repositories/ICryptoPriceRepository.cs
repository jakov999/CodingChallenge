using CodingChallenge.Models;

namespace CodingChallenge.Data.Repositories
{
    public interface ICryptoPriceRepository
    {
        Task<IEnumerable<CryptoPrice>> GetAllPricesAsync();
        Task<IEnumerable<CryptoPrice>> GetLatestPricesAsync();
        Task<IEnumerable<CryptoPrice>> GetPricesByCurrencyAsync(string currency);
        Task AddCryptoPriceAsync(CryptoPrice cryptoPrice);
    }
}
