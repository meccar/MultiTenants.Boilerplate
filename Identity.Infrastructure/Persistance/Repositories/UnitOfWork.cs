using BuildingBlocks.Core.Seedwork.Interface;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistance.Data;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Persistance.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private bool _disposed;
        
        private readonly UserManager<UsersEntity> _userManager;
        private readonly RoleManager<RolesEntity> _roleManager;
        private readonly SignInManager<UsersEntity>? _signInManager;

        public UnitOfWork(
            AppDbContext context,
            UserManager<UsersEntity> userManager,
            RoleManager<RolesEntity> roleManager,
            SignInManager<UsersEntity> signInManager
        ){
            _context = context ??
                       throw new ArgumentNullException(nameof(context));
            _userManager = userManager ??
                           throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ??
                           throw new ArgumentNullException(nameof(roleManager));
            _signInManager = signInManager ??
                             throw new ArgumentNullException(nameof(signInManager));
        }
        

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task BeginTransactionAsync(
            CancellationToken cancellationToken = default)
                => await _context.Database.BeginTransactionAsync(cancellationToken);

        public async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
                => await _context.SaveChangesAsync(cancellationToken);
        
        public async Task CommitTransactionAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);
                await _context.Database.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                await DisposeTrancsactionAsync();
            }
        }

        public async Task RollbackTransactionAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.Database.RollbackTransactionAsync(cancellationToken);
            }
            finally
            {
                await DisposeTrancsactionAsync();
            }
        }

        private async Task DisposeTrancsactionAsync()
        {
            var currentTransaction = _context.Database.CurrentTransaction;
            if (currentTransaction != null)
                await currentTransaction.DisposeAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
                _context.Dispose();
            _disposed = true;
        }
    }

}
