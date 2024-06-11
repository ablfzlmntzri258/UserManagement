using UserManagement.Shared.Models;

namespace UserManagement.Repository.Repositories
{
    public interface IUserRepository: IRepository<User>
    {
        User AuthenticateUser(string username, string password);
        List<int> CheckEmployeeCodes(List<int> employeeCodes);
    }
}
