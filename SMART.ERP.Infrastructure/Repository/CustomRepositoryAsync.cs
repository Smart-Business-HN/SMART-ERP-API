using Ardalis.Specification.EntityFrameworkCore;
using SMART.ERP.Application.Repository;

namespace SMART.ERP.Infrastructure.Repository
{
    public class CustomRepositoryAsync<T> : RepositoryBase<T>, IRepositoryAsync<T> where T : class
    {
        private readonly DatabaseContext db;

        public CustomRepositoryAsync(DatabaseContext db) : base(db)
        {
            this.db = db;
        }
    }
}
