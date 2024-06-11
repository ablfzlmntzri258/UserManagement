using UserManagement.Shared.Models;

namespace UserManagement.Repository.Repositories

{
    public interface IRepository<T> where T : EntityBase
    {
        List<T> GetAll();
        T GetById(int id);
        Task Create(T entity);
        Task Update(T entity);
        Task Delete(int id);
    }
}
