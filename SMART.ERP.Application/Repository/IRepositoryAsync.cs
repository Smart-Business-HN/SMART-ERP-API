using Ardalis.Specification;

namespace SMART.ERP.Application.Repository
{
    public interface IRepositoryAsync<T> : IRepositoryBase<T> where T: class
    {

    }

    public interface IReadRepositoryAsync<T>: IReadRepositoryBase<T> where T: class
    {

    }
}
