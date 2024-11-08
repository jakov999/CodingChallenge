using CodingChallenge.Models;

namespace CodingChallenge.Interfaces
{
    public interface ICryptoPriceRepository : IRepository<CryptoPrice>
    {
        Task<IEnumerable<CryptoPrice>> GetAllPricesAsync();
        Task<IEnumerable<CryptoPrice>> GetLatestPricesAsync();
        Task<IEnumerable<CryptoPrice>> GetPricesByCurrencyAsync(string currency);
    }
}
