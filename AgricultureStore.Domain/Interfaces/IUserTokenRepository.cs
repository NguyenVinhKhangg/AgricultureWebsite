using AgricultureStore.Domain.Entities;

namespace AgricultureStore.Domain.Interfaces
{
    public interface IUserTokenRepository : IRepository<UserToken>
    {
        Task<UserToken?> GetByTokenAsync(string token, TokenType tokenType);
        Task<UserToken?> GetActiveTokenAsync(int userId, TokenType tokenType);
        Task<IEnumerable<UserToken>> GetUserTokensAsync(int userId, TokenType? tokenType = null);
        Task<IEnumerable<UserToken>> GetActiveRefreshTokensAsync(int userId);
        Task RevokeAllUserTokensAsync(int userId, TokenType tokenType);
        Task RevokeTokenAsync(string token);
        Task MarkTokenAsUsedAsync(string token);
        Task CleanupExpiredTokensAsync();
    }
}
