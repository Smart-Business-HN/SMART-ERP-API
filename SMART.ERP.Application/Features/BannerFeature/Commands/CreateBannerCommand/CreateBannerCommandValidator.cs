using FluentValidation;

namespace SMART.ERP.Application.Features.BannerFeature.Commands.CreateBannerCommand
{
    public class CreateBannerCommandValidator : AbstractValidator<CreateBannerCommand>
    {
        public CreateBannerCommandValidator()
        {
            RuleFor(p => p.Url)
                .NotEmpty().WithMessage("El campo {PropertyName} es requerido");

            RuleFor(p => p.FileName)
                .NotEmpty().WithMessage("El campo {PropertyName} es requerido");

            RuleFor(p => p.CompanyId)
                .NotEmpty().WithMessage("El campo {PropertyName} es requerido");
        }
    }
}
