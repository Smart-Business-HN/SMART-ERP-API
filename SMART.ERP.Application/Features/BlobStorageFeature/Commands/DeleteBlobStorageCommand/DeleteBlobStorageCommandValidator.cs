using FluentValidation;

namespace SMART.ERP.Application.Features.BlobStorageFeature.Commands.DeleteBlobStorageCommand
{
    public class DeleteBlobStorageCommandValidator : AbstractValidator<DeleteBlobStorageCommand>
    {
        public DeleteBlobStorageCommandValidator()
        {
            RuleFor(p => p.FileName)
                .NotEmpty().WithMessage("{PropertyName} no puede ser vacio");
        }
    }
}
