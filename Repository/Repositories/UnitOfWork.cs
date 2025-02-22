using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public Task BeginTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public Task CommitTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task RollbackTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
