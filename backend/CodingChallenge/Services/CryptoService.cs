using CodingChallenge.Data.Repositories;
using CodingChallenge.Models;

namespace CodingChallenge.Services
{
    public class CryptoService : ICryptoPriceService
    {
        private readonly ICryptoPriceRepository _cryptoPriceRepository;

        public CryptoService(ICryptoPriceRepository cryptoPriceRepository)
        {
            _cryptoPriceRepository = cryptoPriceRepository;
        }

        public async Task<IEnumerable<CryptoPrice>> GetAllPricesAsync()
        {
            return await _cryptoPriceRepository.GetAllPricesAsync();
        }

        public async Task<IEnumerable<CryptoPrice>> GetLatestPricesAsync()
        {
            return await _cryptoPriceRepository.GetLatestPricesAsync();
        }

        public async Task<IEnumerable<CryptoPrice>> GetPricesByCurrencyAsync(string currency)
        {
            return await _cryptoPriceRepository.GetPricesByCurrencyAsync(currency);
        }

        public async Task AddCryptoPriceAsync(CryptoPrice cryptoPrice)
        {
            await _cryptoPriceRepository.AddCryptoPriceAsync(cryptoPrice);
        }
    }
}
