using FluentValidation;
using UserManagement.Pages.Login;

namespace UserManagement.Validators
{
    public class LoginFluentValidator : AbstractValidator<LoginForm>
    {
        public LoginFluentValidator()
        {
            RuleFor(x => x.username)
                .NotEmpty().WithMessage("نام کاربری نمیتواند خالی باشد!");
            RuleFor(x => x.password)
                .NotEmpty().WithMessage("رمز عبور نمیتواند خالی باشد!")
                .MinimumLength(8).WithMessage("رمز عبور باید حداقل هشت کاراکتر باشد!");
        }


        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<LoginForm>.CreateWithOptions((LoginForm)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
