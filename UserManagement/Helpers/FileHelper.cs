using UserManagement.Shared.Models;

namespace UserManagement.Helpers
{
    public class FileHelper
    {
        private readonly IWebHostEnvironment _environment;
        public FileHelper(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public List<FileModel> GetFiles()
        {
            try
            {
                string[] allFiles = Directory.GetFiles($"{_environment.ContentRootPath}\\FinancialFiles", "*.*", SearchOption.AllDirectories);
                return allFiles.Select(e =>
                {
                    string fileName = e.Split("\\").Last();
                    string[] fileInfo = fileName.Replace(".pdf", "").Split("-");
                    return new FileModel()
                    {
                        Name = fileName,
                        EmployeeCode = int.Parse(fileInfo[0]),
                        Year = int.Parse(fileInfo[1]),
                        Month = int.Parse(fileInfo[2])
                    };
                }).ToList();
            }
            catch
            {
                return new();
            }
            
        }

        public List<FileModel> GetUserFiles(int employeeCode)
        {
            try
            {
                string[] allFiles = Directory.GetFiles($"{_environment.ContentRootPath}\\FinancialFiles\\{employeeCode}", "*.*", SearchOption.AllDirectories);
                return allFiles.Select(e =>
                {
                    string fileName = e.Split("\\").Last();
                    string[] fileInfo = fileName.Replace(".pdf", "").Split("-");
                    return new FileModel()
                    {
                        Name = fileName,
                        Year = int.Parse(fileInfo[1]),
                        Month = int.Parse(fileInfo[2])
                    };
                }).ToList();
            }
            catch 
            {
                return new();
            }
        }
    }
}
