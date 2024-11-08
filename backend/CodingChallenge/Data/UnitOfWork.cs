using CodingChallenge.Data.Repositories;
using CodingChallenge.Interfaces;

namespace CodingChallenge.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public ICryptoPriceRepository CryptoPriceRepository { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            CryptoPriceRepository = new CryptoPriceRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
