using Microsoft.EntityFrameworkCore;
using UserManagement.Repository.Contexts;
using UserManagement.Shared.Models;

namespace UserManagement.Repository.Repositories
{
    class UserRepository: IUserRepository
    {
        private readonly UserContext _context;
        public UserRepository(UserContext context)
        {
            _context = context;
        }

        public List<User> GetAll()
        {
            return _context.Users.AsNoTracking().ToList();
        }

        public User GetById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id.Equals(id));
        }

        
        public User AuthenticateUser(string username, string password)
        {
            return _context.Users.FirstOrDefault(u => u.UserName.Equals(username) && u.Password.Equals(password));
        }
        

        public async Task Create(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task Update(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var existingUser = _context.Users.First(u => u.Id.Equals(id));
            _context.Users.Remove(existingUser);
            await _context.SaveChangesAsync();
        }

        public List<int> CheckEmployeeCodes(List<int> employeeCodes)
        {
            List<int> allCodes = _context.Users.Select(u => u.EmployeeCode).ToList();
            return employeeCodes.Where(ec => allCodes.Contains(ec)).ToList();

        }

        public bool CheckDuplicateUsername(User user, bool isUpdating)
        {
            if (isUpdating)
            {
                return _context.Users.Where(u => u.Id != user.Id).Any(u => u.UserName == user.UserName);
            }
            return _context.Users.Any(u => u.UserName == user.UserName);
        }

        public bool CheckDuplicateEmployeeCode(User user, bool isUpdating)
        {
            if (isUpdating)
            {
                return _context.Users.Where(u => u.Id != user.Id).Any(u => u.EmployeeCode == user.EmployeeCode);
            }
            return _context.Users.Any(u => u.EmployeeCode == user.EmployeeCode);
        }

        public bool CheckDuplicateEmail(User user, bool isUpdating)
        {
            if (isUpdating)
            {
                return _context.Users.Where(u => u.Id != user.Id).Any(u => u.Email == user.Email);
            }
            return _context.Users.Any(u => u.Email == user.Email);
        }
    }
}
