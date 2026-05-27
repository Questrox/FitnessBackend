using Domain.Interfaces;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfWork(FitnessDb _db) : IUnitOfWork
    {
        public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

            return new EfTransaction(transaction);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
