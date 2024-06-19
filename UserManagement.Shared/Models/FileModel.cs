using Microsoft.AspNetCore.Components.Forms;

namespace UserManagement.Shared.Models;

public class FileModel
{
    public int EmployeeCode { get; set; }
    public int Year { get; set; }

    public int Month { get; set; }

    public IBrowserFile? File { get; set; }

    public string? Name { get; set; }
    public string? Code_Name { get; set; }

}