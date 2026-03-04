using FluentValidation;

namespace SMART.ERP.Application.Features.ProjectAttachmentFeature.Commands.DeleteProjectAttachmentCommand
{
    public class DeleteProjectAttachmentCommandValidator : AbstractValidator<DeleteProjectAttachmentCommand>
    {
        public DeleteProjectAttachmentCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
