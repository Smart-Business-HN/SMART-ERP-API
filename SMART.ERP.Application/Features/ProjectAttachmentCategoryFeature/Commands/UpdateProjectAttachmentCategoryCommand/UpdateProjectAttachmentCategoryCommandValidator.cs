using FluentValidation;

namespace SMART.ERP.Application.Features.ProjectAttachmentCategoryFeature.Commands.UpdateProjectAttachmentCategoryCommand
{
    public class UpdateProjectAttachmentCategoryCommandValidator : AbstractValidator<UpdateProjectAttachmentCategoryCommand>
    {
        public UpdateProjectAttachmentCategoryCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("{PropertyName} es requerido")
                .MaximumLength(100).WithMessage("{PropertyName} no debe exceder {MaxLength} caracteres");
        }
    }
}
