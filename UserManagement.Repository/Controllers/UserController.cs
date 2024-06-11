using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using UserManagement.Repository.Repositories;
using UserManagement.Shared.Models;

namespace UserManagement.Repository.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly IUserRepository userRepository;
    private readonly ILogger<UserController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserController(IUserRepository userRepository, ILogger<UserController> logger,IHttpContextAccessor httpContextAccessor)
    {
        this.userRepository = userRepository;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet("all")]
    public IActionResult GetAllCustomers()
    {
        return Ok(userRepository.GetAll());
    }
        
    [HttpGet]
    public IActionResult GetById()
    {
        var userId = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "-1"; 
        return Ok(userRepository.GetById(int.Parse(userId)));
    }

    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody]User user)
    {
        try
        {
            await userRepository.Create(user);
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
            {
                if (sqlEx.Message.Contains("IX_t_user_UserName"))
                {
                    return BadRequest("A user with this username already exists");
                }
                else if (sqlEx.Message.Contains("IX_t_user_Email"))
                {
                    return BadRequest("A user with this email already exists");
                }
            }
            else
            {
                throw;
            }
        }
        return CreatedAtAction(nameof(Create), new { id = user.Id }, user);
    }
        
    [HttpPut]
    public async Task<IActionResult> Update(User user)
    {
        try
        {
            await userRepository.Update(user);
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
            {
                if (sqlEx.Message.Contains("IX_t_user_UserName"))
                {
                    return BadRequest("A user with this username already exists");
                }
                else if (sqlEx.Message.Contains("IX_t_user_Email"))
                {
                    return BadRequest("A user with this email already exists");
                }
            }
            else
            {
                throw;
            }
        }
        return NoContent();
    }

    [HttpDelete("{userId:int}")]
    public IActionResult Delete(int userId)
    {
        User userCategory = userRepository.GetById(userId);
        if (userCategory == null)
        {
            return NotFound();
        }
        userRepository.Delete(userId);
        return NoContent();
    }

    [HttpPost("check-codes")]
    public async Task<IActionResult> CheckEmployeeCodes([FromBody] List<int> employeeCodes)
    {
        return Ok(userRepository.CheckEmployeeCodes(employeeCodes));
    }


}