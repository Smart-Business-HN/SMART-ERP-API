using FluentValidation;

namespace SMART.ERP.Application.Features.ChatFeature.Commands.CreateChatSessionCommand
{
    public class CreateChatSessionCommandValidator : AbstractValidator<CreateChatSessionCommand>
    {
        public CreateChatSessionCommandValidator()
        {
            RuleFor(p => p.SessionIdentifier)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(36).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.VisitorName)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(100).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.VisitorEmail)
                .EmailAddress().WithMessage("Verifique el formato de {PropertyName}")
                .When(p => !string.IsNullOrEmpty(p.VisitorEmail));
        }
    }
}
