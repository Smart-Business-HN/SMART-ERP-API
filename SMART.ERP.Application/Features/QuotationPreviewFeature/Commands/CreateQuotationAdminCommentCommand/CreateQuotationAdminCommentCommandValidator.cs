using FluentValidation;

namespace SMART.ERP.Application.Features.QuotationPreviewFeature.Commands.CreateQuotationAdminCommentCommand
{
    public class CreateQuotationAdminCommentCommandValidator : AbstractValidator<CreateQuotationAdminCommentCommand>
    {
        public CreateQuotationAdminCommentCommandValidator()
        {
            RuleFor(p => p.QuotationId)
                .GreaterThan(0).WithMessage("{PropertyName} debe ser mayor a 0");

            RuleFor(p => p.Message)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(600).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.UserId)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
