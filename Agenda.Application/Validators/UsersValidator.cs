using Agenda.Core.QueryFilters;
using FluentValidation;

namespace Agenda.Application.Validators;

public class UsersValidator : AbstractValidator<UsersQueryFilter>
{
    public UsersValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre debe tener máximo 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no es válido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(6).WithMessage("La contraseña debe tener mínimo 6 caracteres.");
    }
}