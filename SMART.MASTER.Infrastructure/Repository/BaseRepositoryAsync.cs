using Ardalis.Specification.EntityFrameworkCore;
using SMART.ERP.Application.Repository;
using SMART.MASTER.Infrastructure.Context;

namespace SMART.MASTER.Infrastructure.Repository
{
    public class BaseRepositoryAsync<T> : RepositoryBase<T>, IRepositoryHNAsync<T> where T : class
    {
        private readonly BaseContext db;

        public BaseRepositoryAsync(BaseContext db) : base(db)
        {
            this.db = db;
        }
    }
}
