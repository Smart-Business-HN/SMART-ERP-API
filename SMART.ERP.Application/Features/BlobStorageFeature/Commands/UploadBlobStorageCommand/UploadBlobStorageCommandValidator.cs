using FluentValidation;

namespace SMART.ERP.Application.Features.BlobStorageFeature.Commands.UploadBlobStorageCommand
{
    public class UploadBlobStorageCommandValidator : AbstractValidator<UploadBlobStorageCommand>
    {
        public UploadBlobStorageCommandValidator()
        {
            RuleFor(x => x.File.Length)
                .NotNull()
                .LessThanOrEqualTo(10000000).WithMessage("El tamaño del archivo no debe exceder 10MB");

            RuleFor(x => x.File.ContentType)
                .NotNull()
                .Must(x =>
                x.Equals("image/jpeg")
                || x.Equals("image/jpg")
                || x.Equals("image/png")
                || x.Equals("application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                || x.Equals("application/pdf")
                || x.Equals("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                || x.Equals("application/vnd.ms-excel"))
                .WithMessage("Tipo de archivo no permitido");
        }
    }
}
