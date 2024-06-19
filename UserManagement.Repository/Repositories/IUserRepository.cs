using Microsoft.EntityFrameworkCore;
using UserManagement.Shared.Models;

namespace UserManagement.Repository.Repositories
{
    public interface IUserRepository: IRepository<User>
    {
        User AuthenticateUser(string username, string password);
        List<int> CheckEmployeeCodes(List<int> employeeCodes);
        public bool CheckDuplicateUsername(User user, bool isUpdating);
        public bool CheckDuplicateEmployeeCode(User user, bool isUpdating);
        public bool CheckDuplicateEmail(User user, bool isUpdating);
        
    }
}
