using FluentValidation;

namespace SMART.ERP.Application.Features.AdCampaignFeature.Commands.SendAdCampaignCommand
{
    public class SendAdCampaignCommandValidator : AbstractValidator<SendAdCampaignCommand>
    {
        public SendAdCampaignCommandValidator()
        {
            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("{PropertyName} es requerido");

            RuleFor(x => x.File)
                .Must(x => x!.Count <= 3).WithMessage("No se permiten mas de tres archivos").When(x => x.File != null);

            RuleForEach(x => x.File)
                .Must(file => file.Length < 4000000).WithMessage("Solamente se permiten archivos igual o menor a 4MB")
                .Must(file => file.ContentType == "application/pdf" || file.ContentType == "image/png" || file.ContentType == "image/jpeg").WithMessage("Formato de archivo no permitido")
                .When(x => x.File != null);

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("{PropertyName} es requerido");
        }
    }
}
