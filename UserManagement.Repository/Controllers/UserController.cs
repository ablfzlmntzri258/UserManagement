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

    public UserController(IUserRepository userRepository, ILogger<UserController> logger, IHttpContextAccessor httpContextAccessor)
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
    public async Task<IActionResult> Create([FromBody] User user)
    {
        try
        {
            if (userRepository.CheckDuplicateEmployeeCode(user, false))
            {
                return BadRequest(new ApiResponse<User>
                {
                    Success = false,
                    ErrorMessage = "کاربری با این کد از قبل وجود دارد"
                });

            }
            if (userRepository.CheckDuplicateEmail(user, false))
            {
                return BadRequest(new ApiResponse<User>
                {
                    Success = false,
                    ErrorMessage = "کاربری با این ایمیل از قبل وجود دارد"
                });

            }
            if (userRepository.CheckDuplicateUsername(user, false))

            {
                return BadRequest(new ApiResponse<User>
                {
                    Success = false,
                    ErrorMessage = "کاربری با این نام کاربری از قبل وجود دارد"
                });

            }
            await userRepository.Create(user);
            return CreatedAtAction(nameof(Create), new { id = user.Id }, new ApiResponse<User>
            {
                Success = true,
                Data = user
            });
        }
        catch
        {
            return BadRequest(new ApiResponse<User>
            {
                Success = false,
                ErrorMessage = "Unexpected Error"
            });
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update(User user)
    {
        try
        {
            if (userRepository.CheckDuplicateEmployeeCode(user, true)) {
                return BadRequest(new ApiResponse<User>
                {
                    Success = false,
                    ErrorMessage = "کاربری با این کد از قبل وجود دارد"
                });
                
            }
            if (userRepository.CheckDuplicateEmail(user, true))
            {
                return BadRequest(new ApiResponse<User>
                {
                    Success = false,
                    ErrorMessage = "کاربری با این ایمیل از قبل وجود دارد"
                });

            }
            if (userRepository.CheckDuplicateUsername(user, true))

            {
                return BadRequest(new ApiResponse<User>
                {
                    Success = false,
                    ErrorMessage = "کاربری با این نام کاربری از قبل وجود دارد"
                });

            }
            await userRepository.Update(user);
            return Ok(new ApiResponse<User>
            {
                Success = true,
                Data = user
            });
        }
        catch
        {
            return Ok(new ApiResponse<User>
            {
                Success = false,
                ErrorMessage = "خطای غیرمنتظره"
            });
        }
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

    
    [HttpPost("changepassword")]
    public async Task<IActionResult> ChangePassword(ChangePasswordForm passwordForm)
    {
        var userId = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "-1";
        User user = userRepository.GetById(int.Parse(userId));
        if (passwordForm.OldPass != user.Password)
        {
            return BadRequest(new ApiResponse()
            {
                Success = false,
                ErrorMessage = "رمز قدیمی اشتباه است"
            });
        }

        if (passwordForm.NewPass != passwordForm.NewPassConfirmation)
        {
            return BadRequest(new ApiResponse()
            {
                Success = false,
                ErrorMessage = "رمز جدید با تایید آن برابر نیست"
            });
        }

        user.Password = passwordForm.NewPass;
        await userRepository.Update(user);
        return Ok(new ApiResponse()
        {
            Success = true,
        });
    }

}