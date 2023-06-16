using FluentValidation;

namespace SMART.ERP.Application.Features.MessageFeature.Commands.SendMessageCommand
{
    public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
    {
        public SendMessageCommandValidator()
        {
            RuleFor(x => x.Attachments)
                .NotEmpty().WithMessage("{PropertyName} debe contener al menos un archivo")
                .Must(x => x!.Count <= 3).WithMessage("{PropertyName} no deben contener mas de tres archivos")
                .When(x => x.Attachments != null);

            RuleForEach(x => x.Attachments)
                .Must(x => x.Length <= 4000000).WithMessage("Solamente se permiten archivos menor o igual a 4MB")
                .Must(file => file.ContentType == "application/pdf" || file.ContentType == "image/png" || file.ContentType == "image/jpeg").WithMessage("Formato de archivo no permitido")
                .When(x => x.Attachments != null);

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(100).WithMessage("{PropertyName} no debe contener mas de {MaxLength} caracteres");

            RuleFor(x => x.MessageContent)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(2000).WithMessage("{PropertyName} no debe contener mas de {MaxLength} caracteres");

            RuleFor(x => x.ReceptorId)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .NotNull().WithMessage("{PropertyName} es requerido");
        }
    }
}
