using System.Text.RegularExpressions;
using FluentValidation;
using UserManagement.Shared.Models;

namespace UserManagement.Validators;

public class FileModelFluentValidator : AbstractValidator<FileModel>
{
    public FileModelFluentValidator()
    {
        RuleFor(x => x.EmployeeCode).NotEmpty();
        RuleFor(x => x.Year).NotEmpty().
            InclusiveBetween(1390, 1500).WithMessage("Year must be between 1390 and 1500");
        RuleFor(x => x.Month).NotEmpty().
            InclusiveBetween(1, 12).WithMessage("Month must be between 1 and 12");
        RuleFor(x => x.File)
            .NotEmpty().WithMessage("You must select a file first");
        When(x => x.File != null, () =>
        {
            RuleFor(x => x.File.Size).LessThanOrEqualTo(10485760).WithMessage("The maximum file size is 10 MB");
        });
    }

    public bool IsFileNameValid(string value)
    {
        var regex = new Regex(@"^(1[0-9]*|[2-9][0-9]*)-(13[9][0-9]|14[0-9]{2}|1500)-([1-9]|1[0-2])$");
        return regex.IsMatch(value);
    }
    
    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<FileModel>.CreateWithOptions((FileModel)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}