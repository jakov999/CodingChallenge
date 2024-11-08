using CodingChallenge.Interfaces;
using CodingChallenge.Models;

namespace CodingChallenge.Services
{
    public class CryptoService : ICryptoPriceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CryptoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CryptoPrice>> GetAllPricesAsync()
        {
            return await _unitOfWork.CryptoPriceRepository.GetAllPricesAsync();
        }

        public async Task<IEnumerable<CryptoPrice>> GetLatestPricesAsync()
        {
            return await _unitOfWork.CryptoPriceRepository.GetLatestPricesAsync();
        }

        public async Task<IEnumerable<CryptoPrice>> GetPricesByCurrencyAsync(string currency)
        {
            return await _unitOfWork.CryptoPriceRepository.GetPricesByCurrencyAsync(currency);
        }
    }
}
