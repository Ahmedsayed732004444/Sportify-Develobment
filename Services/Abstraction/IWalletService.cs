using Sportiva.Abstractions;

namespace Sportiva.Services;

/// <summary>
/// Service for managing wallet/credit operations including deductions and refunds.
/// </summary>
public interface IWalletService
{
    /// <summary>
    /// Deducts an amount from the user's wallet balance.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="amount">The amount to deduct (must be positive).</param>
    /// <param name="reason">The reason for the deduction (e.g., "Subscription.SubscribeAsync", "Subscription.RenewAsync").</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// Success if deduction was processed. 
    /// Failure with "Wallet.InsufficientBalance" (402) if balance is insufficient.
    /// </returns>
    Task<Result> DeductAsync(string userId, decimal amount, string reason, CancellationToken ct = default);

    /// <summary>
    /// Credits/refunds an amount to the user's wallet balance.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="amount">The amount to credit (must be positive).</param>
    /// <param name="reason">The reason for the credit (e.g., "Subscription.CancelAsync refund").</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Success if credit was processed.</returns>
    Task<Result> CreditAsync(string userId, decimal amount, string reason, CancellationToken ct = default);

    /// <summary>
    /// Gets the current wallet balance for a user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The current balance, or Failure if user not found.</returns>
    Task<Result<decimal>> GetBalanceAsync(string userId, CancellationToken ct = default);
}
