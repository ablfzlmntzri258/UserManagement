using FluentValidation;
using UserManagement.Shared.Models;

namespace UserManagement.Validators;

public class UserFluentValidator : AbstractValidator<User>
{
    public UserFluentValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("وارد کردن نام اجباری است");
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("وارد کردن نام کاربری اجباری است");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("وارد کردن رمز ضروری است")
            .MinimumLength(8).WithMessage("رمز حداقل باید هشت کاراکتر باشد");
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("وارد کردن ایمیل ضروری است")
            .EmailAddress().WithMessage("ایمیل وارد شده معتبر نیست");
        RuleFor(x => x.EmployeeCode)
            .NotEmpty().WithMessage("وارد کردن کد کارمند ضروری است");
        RuleFor(x => x.Permission)
            .NotEmpty().WithMessage("انتخاب نقش ضروری است");
    }


    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<User>.CreateWithOptions((User)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}