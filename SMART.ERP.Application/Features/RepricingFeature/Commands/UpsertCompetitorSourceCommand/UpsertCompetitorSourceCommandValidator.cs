using FluentValidation;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.RepricingFeature.Commands.UpsertCompetitorSourceCommand
{
    public class UpsertCompetitorSourceCommandValidator : AbstractValidator<UpsertCompetitorSourceCommand>
    {
        public UpsertCompetitorSourceCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("El producto es requerido");

            RuleFor(x => x.CompetitorName)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(50).WithMessage("{PropertyName} no debe exceder los {MaxLength} caracteres");

            RuleFor(x => x.ProductUrl)
                .NotEmpty().WithMessage("La URL del competidor es requerida");

            RuleFor(x => x.PriceSelector)
                .NotEmpty()
                .When(x => x.ParseStrategy == ParseStrategy.HtmlCssSelector || x.ParseStrategy == ParseStrategy.Headless)
                .WithMessage("El selector CSS es requerido para esta estrategia de lectura");

            RuleFor(x => x.ManualPrice)
                .GreaterThan(0)
                .When(x => x.ParseStrategy == ParseStrategy.Manual && x.ManualPrice.HasValue)
                .WithMessage("El precio manual debe ser mayor a 0");
        }
    }
}
