namespace Sportiva.Services.Implementation
{
    public class WalletService : IWalletService
    {
        public Task<Result> CreditAsync(string userId, decimal amount, string reason, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeductAsync(string userId, decimal amount, string reason, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result<decimal>> GetBalanceAsync(string userId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
