using FluentValidation;

namespace SMART.ERP.Application.Features.ChatFeature.Commands.SendChatMessageCommand
{
    public class SendChatMessageCommandValidator : AbstractValidator<SendChatMessageCommand>
    {
        public SendChatMessageCommandValidator()
        {
            RuleFor(p => p.ChatSessionId)
                .GreaterThan(0).WithMessage("{PropertyName} debe ser mayor a 0");

            RuleFor(p => p.Content)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .MaximumLength(2000).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");

            RuleFor(p => p.SenderType)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio")
                .Must(x => x == "visitor" || x == "admin")
                .WithMessage("{PropertyName} debe ser 'visitor' o 'admin'");

            RuleFor(p => p.SenderName)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio");

            RuleFor(p => p.SessionIdentifier)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio");
        }
    }
}
