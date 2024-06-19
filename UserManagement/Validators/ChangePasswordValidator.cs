using FluentValidation;
using UserManagement.Shared.Models;

namespace UserManagement.Validators;

public class ChangePasswordFluentValidator : AbstractValidator<ChangePasswordForm>
{
    public ChangePasswordFluentValidator()
    {
        RuleFor(x => x.OldPass)
            .NotEmpty().WithMessage("فیلد اجباری").MinimumLength(8).WithMessage("حداقل هشت کاراکتر");
        RuleFor(x => x.NewPass)
            .NotEmpty().WithMessage("فیلد اجباری").MinimumLength(8).WithMessage("حداقل هشت کاراکتر");
        RuleFor(x => x.NewPassConfirmation)
            .NotEmpty().WithMessage("فیلد اجباری").MinimumLength(8).WithMessage("حداقل هشت کاراکتر")
            .Equal(x => x.NewPass).WithMessage("رمز جدید و تایید آن منطبق نیستند");
    }


    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<ChangePasswordForm>.CreateWithOptions((ChangePasswordForm)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}