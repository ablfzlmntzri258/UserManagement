using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace UserManagement.Shared.Models;

[Table("t_user")]
public class User : EntityBase
{
    
    public string Name { get; set; }
    
    public string UserName { get; set; }
    
    public string Password { get; set; }

    public string Email { get; set; }

    public int EmployeeCode { get; set; }
    
    [Column(TypeName = "tinyint")]
    public int Permission { get; set; }

    [NotMapped]
    public string? Role { get; set; }
    
    public DateTime CreateAt { get; set; }
    
    public string? RefreshToken { get; set; }
    
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public ClaimsPrincipal ToClaimsPrincipal() => new(new ClaimsIdentity(new Claim[]
        {
            new (ClaimTypes.Name, UserName),
            new (ClaimTypes.NameIdentifier, Id.ToString()),
            new (ClaimTypes.Email, Email),
            new (ClaimTypes.Role, Permission.ToString() == "1" ? "admin" : Permission.ToString() == "2" 
                ? "financial" : "employee"),
            new (nameof(EmployeeCode), EmployeeCode.ToString())
        },"Custom"));

    // public static User FromClaimsPrincipal(ClaimsPrincipal principal) => new()
    // {
    //     Name = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "",
    //     UserName = principal.FindFirst(ClaimTypes.Name)?.Value ?? "",
    //     Password = principal.FindFirst(ClaimTypes.Hash)?.Value ?? "",
    // };
}