using FluentValidation;

namespace SMART.ERP.Application.Features.EcommerceUserFeature.Queries.GetEcommerceUserByIdQuery;

public class GetEcommerceUserByIdQueryValidator : AbstractValidator<GetEcommerceUserByIdQuery>
{
    public GetEcommerceUserByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El Id es requerido.");
    }
}