using FluentValidation;
using UserManagement.Shared.Models;

namespace UserManagement.Validators;

public class UserFluentValidator : AbstractValidator<User>
{
    public UserFluentValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required!");
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required!")
            .MinimumLength(8).WithMessage("Username must at least have 8 characters");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required!")
            .MinimumLength(8).WithMessage("Password must at least have 8 characters");
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required!")
            .EmailAddress().WithMessage("You must provide a email address!");
        RuleFor(x => x.EmployeeCode)
            .NotEmpty().WithMessage("EmployeeCode is required!");
        RuleFor(x => x.Permission)
            .NotEmpty().WithMessage("Permission is required!");
    }


    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<User>.CreateWithOptions((User)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}