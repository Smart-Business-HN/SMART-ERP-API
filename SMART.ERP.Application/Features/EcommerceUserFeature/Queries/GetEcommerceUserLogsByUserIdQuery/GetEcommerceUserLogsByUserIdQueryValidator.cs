using FluentValidation;

namespace SMART.ERP.Application.Features.EcommerceUserFeature.Queries.GetEcommerceUserLogsByUserIdQuery;

public class GetEcommerceUserLogsByUserIdQueryValidator : AbstractValidator<GetEcommerceUserLogsByUserIdQuery>
{
    public GetEcommerceUserLogsByUserIdQueryValidator()
    {
        RuleFor(x => x.EcommerceUserId)
            .NotEmpty().WithMessage("El Id del usuario es requerido.");
    }
}
