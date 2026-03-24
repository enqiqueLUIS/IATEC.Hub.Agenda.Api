using Agenda.Core.QueryFilters;
using FluentValidation;

namespace Agenda.Application.Validators;

public class EventsValidator : AbstractValidator<EventsQueryFilter>
{
    public EventsValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título es obligatorio.")
            .MaximumLength(200).WithMessage("El título debe tener máximo 200 caracteres.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("La fecha de inicio es obligatoria.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("La fecha de fin es obligatoria.")
            .GreaterThan(x => x.StartDate).WithMessage("La fecha de fin debe ser mayor a la fecha de inicio.");

        RuleFor(x => x.EventType)
            .NotEmpty().WithMessage("El tipo de evento es obligatorio.")
            .Must(t => t == "Exclusive" || t == "Shared")
            .WithMessage("El tipo de evento debe ser 'Exclusive' o 'Shared'.");

        RuleFor(x => x.Location)
            .Must(loc => string.IsNullOrWhiteSpace(loc) || IsGoogleMapsUrl(loc))
            .WithMessage("El lugar debe ser un link válido de Google Maps.");
    }

    private static bool IsGoogleMapsUrl(string url) =>
        url.StartsWith("https://www.google.com/maps") ||
        url.StartsWith("https://maps.google.com") ||
        url.StartsWith("https://goo.gl/maps") ||
        url.StartsWith("https://maps.app.goo.gl");
}