using FluentValidation;

namespace SMART.ERP.Application.Features.QuoteProductFeature.Commands.UpdateStatusQuoteProductCommand
{
    public class UpdateStatusQuoteProductCommandValidator : AbstractValidator<UpdateStatusQuoteProductCommand>
    {
        public UpdateStatusQuoteProductCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
