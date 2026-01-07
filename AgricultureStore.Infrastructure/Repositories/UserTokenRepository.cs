using AgricultureStore.Domain.Entities;
using AgricultureStore.Domain.Interfaces;
using AgricultureStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgricultureStore.Infrastructure.Repositories
{
    public class UserTokenRepository : Repository<UserToken>, IUserTokenRepository
    {
        public UserTokenRepository(AgricultureDbContext context) : base(context)
        {
        }

        public async Task<UserToken?> GetByTokenAsync(string token, TokenType tokenType)
        {
            return await _context.UserTokens
                .Include(ut => ut.User)
                .FirstOrDefaultAsync(ut => ut.Token == token && ut.TokenType == tokenType);
        }

        public async Task<UserToken?> GetActiveTokenAsync(int userId, TokenType tokenType)
        {
            return await _context.UserTokens
                .Where(ut => ut.UserId == userId 
                    && ut.TokenType == tokenType
                    && !ut.IsUsed
                    && ut.RevokedAt == null
                    && ut.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(ut => ut.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<UserToken>> GetUserTokensAsync(int userId, TokenType? tokenType = null)
        {
            var query = _context.UserTokens.Where(ut => ut.UserId == userId);
            
            if (tokenType.HasValue)
            {
                query = query.Where(ut => ut.TokenType == tokenType.Value);
            }
            
            return await query.OrderByDescending(ut => ut.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<UserToken>> GetActiveRefreshTokensAsync(int userId)
        {
            return await _context.UserTokens
                .Where(ut => ut.UserId == userId 
                    && ut.TokenType == TokenType.RefreshToken
                    && !ut.IsUsed
                    && ut.RevokedAt == null
                    && ut.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(ut => ut.CreatedAt)
                .ToListAsync();
        }

        public async Task RevokeAllUserTokensAsync(int userId, TokenType tokenType)
        {
            var tokens = await _context.UserTokens
                .Where(ut => ut.UserId == userId 
                    && ut.TokenType == tokenType
                    && ut.RevokedAt == null)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.RevokedAt = DateTime.UtcNow;
            }
        }

        public async Task RevokeTokenAsync(string token)
        {
            var userToken = await _context.UserTokens.FirstOrDefaultAsync(ut => ut.Token == token);
            if (userToken != null)
            {
                userToken.RevokedAt = DateTime.UtcNow;
            }
        }

        public async Task MarkTokenAsUsedAsync(string token)
        {
            var userToken = await _context.UserTokens.FirstOrDefaultAsync(ut => ut.Token == token);
            if (userToken != null)
            {
                userToken.IsUsed = true;
                userToken.UsedAt = DateTime.UtcNow;
            }
        }

        public async Task CleanupExpiredTokensAsync()
        {
            var expiredTokens = await _context.UserTokens
                .Where(ut => ut.ExpiresAt < DateTime.UtcNow.AddDays(-7)) // Keep expired tokens for 7 days for audit
                .ToListAsync();

            _context.UserTokens.RemoveRange(expiredTokens);
        }
    }
}
