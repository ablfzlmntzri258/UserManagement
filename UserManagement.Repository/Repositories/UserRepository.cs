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
            return _context.Users.ToList();
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
            var existingUser = _context.Users.First(u => u.Id.Equals(user.Id));
            existingUser.UserName = user.UserName;
            existingUser.Name = user.Name;
            existingUser.Password = user.Password;
            existingUser.Email = user.Email;
            existingUser.EmployeeCode = user.EmployeeCode;
            existingUser.RefreshToken = user.RefreshToken;
            existingUser.RefreshTokenExpiryTime = user.RefreshTokenExpiryTime;
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
    }
}
