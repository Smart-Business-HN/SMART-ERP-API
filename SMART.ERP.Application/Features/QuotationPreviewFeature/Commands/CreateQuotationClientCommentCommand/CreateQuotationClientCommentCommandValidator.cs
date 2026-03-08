using FluentValidation;

namespace SMART.ERP.Application.Features.QuotationPreviewFeature.Commands.CreateQuotationClientCommentCommand
{
    public class CreateQuotationClientCommentCommandValidator : AbstractValidator<CreateQuotationClientCommentCommand>
    {
        public CreateQuotationClientCommentCommandValidator()
        {
            RuleFor(p => p.AccessToken)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.AuthorName)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(100).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.AuthorEmail)
                .MaximumLength(150).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres")
                .EmailAddress().When(p => !string.IsNullOrEmpty(p.AuthorEmail))
                .WithMessage("{PropertyName} no es un correo electrónico válido");

            RuleFor(p => p.Message)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(600).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}
