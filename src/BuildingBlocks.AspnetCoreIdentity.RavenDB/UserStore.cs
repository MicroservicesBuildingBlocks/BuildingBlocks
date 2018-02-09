using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace BuildingBlocks.AspnetCoreIdentity.RavenDB
{
    public class UserStore :
        IUserPasswordStore<User>,
        IUserClaimStore<User>
    {

        public IAsyncDocumentSession Session { get; }

        public UserStore(IAsyncDocumentSession session)
        {
            session.Advanced.UseOptimisticConcurrency = true;
            Session = session;
        }

        #region IUserStore

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Name);
        }

        public Task SetUserNameAsync(User user, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            user.Name = name;

            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.NormalizedName);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.NormalizedName = normalizedName ??
                                  throw new ArgumentNullException(nameof(normalizedName));

            return Task.CompletedTask;
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            await Session.StoreAsync(user, cancellationToken);
            await Session.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await Session.StoreAsync(user, cancellationToken);
            await Session.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            Session.Delete(user.Id);
            await Session.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }

        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            cancellationToken.ThrowIfCancellationRequested();
            return Session.LoadAsync<User>(userId, cancellationToken);
        }

        public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return Session.Query<User>().FirstOrDefaultAsync(
                u => u.NormalizedName == normalizedUserName, cancellationToken
            );
        }

        #endregion

        #region IUserPasswordStore

        public Task SetPasswordHashAsync(User user, string passwordHash,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.PasswordHash = passwordHash;

            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(User user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PasswordHash != null);
        }

        #endregion

        #region IDisposable

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        private bool _disposed;

        public void Dispose()
        {
            //Session.Dispose();
            _disposed = true;
        }

        #endregion

        #region IUserClaimStore

        public Task<IList<Claim>> GetClaimsAsync(User user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult<IList<Claim>>(user.Claims.Select(c => new Claim(c.Type, c.Value)).ToList());
        }

        public Task AddClaimsAsync(User user, IEnumerable<Claim> claims,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            foreach (var claim in claims)
            {
                user.AddClaim(claim);
            }

            return Task.CompletedTask;
        }

        public Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            if (newClaim == null)
            {
                throw new ArgumentNullException(nameof(newClaim));
            }

            user.RemoveClaim(claim);
            user.AddClaim(newClaim);

            return Task.CompletedTask;
        }

        public Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            foreach (var claim in claims)
            {
                user.RemoveClaim(claim);
            }

            return Task.CompletedTask;
        }

        public Task<IList<User>> GetUsersForClaimAsync(Claim claim,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            var query =
                from user in Session.Query<User>()
                where user.Claims.Any(c => c.Type == claim.Type && c.Value == claim.Value)
                select user;

            // LIMIT TO 128 RESULTS
            return Task.FromResult((IList<User>)query.ToList());
        }

        #endregion
    }
}
