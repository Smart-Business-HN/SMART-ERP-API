using FluentValidation;

namespace SMART.ERP.Application.Features.ProjectAttachmentCategoryFeature.Commands.DeleteProjectAttachmentCategoryCommand
{
    public class DeleteProjectAttachmentCategoryCommandValidator : AbstractValidator<DeleteProjectAttachmentCategoryCommand>
    {
        public DeleteProjectAttachmentCategoryCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
