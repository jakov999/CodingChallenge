namespace CodingChallenge.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICryptoPriceRepository CryptoPriceRepository { get; }
        Task<int> CompleteAsync();
    }
}
