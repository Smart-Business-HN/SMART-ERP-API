using FluentValidation;
using SMART.ERP.Application.Services.BlobStorageService;

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
                || x.Equals("image/webp")
                || x.Equals("image/gif")
                || x.Equals("application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                || x.Equals("application/pdf")
                || x.Equals("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                || x.Equals("application/vnd.ms-excel"))
                .WithMessage("Tipo de archivo no permitido");

            // La carpeta es opcional; si viene, debe pertenecer a la taxonomía conocida.
            RuleFor(x => x.Folder)
                .Must(BlobFolders.IsValid)
                .When(x => !string.IsNullOrWhiteSpace(x.Folder))
                .WithMessage("Carpeta de destino no válida");
        }
    }
}
