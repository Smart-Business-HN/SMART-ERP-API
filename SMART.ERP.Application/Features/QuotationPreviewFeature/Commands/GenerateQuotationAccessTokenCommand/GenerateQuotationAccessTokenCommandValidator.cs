using FluentValidation;

namespace SMART.ERP.Application.Features.QuotationPreviewFeature.Commands.GenerateQuotationAccessTokenCommand
{
    public class GenerateQuotationAccessTokenCommandValidator : AbstractValidator<GenerateQuotationAccessTokenCommand>
    {
        public GenerateQuotationAccessTokenCommandValidator()
        {
            RuleFor(p => p.QuotationId)
                .GreaterThan(0).WithMessage("{PropertyName} debe ser mayor a 0");
        }
    }
}
