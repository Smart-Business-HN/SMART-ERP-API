using FluentValidation;

namespace SMART.ERP.Application.Features.BannerFeature.Commands.DeleteBannerCommand
{
    public class DeleteBannerCommandValidator : AbstractValidator<DeleteBannerCommand>
    {
        public DeleteBannerCommandValidator()
        {
            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("El campo {PropertyName} es requerido");
        }
    }
}
