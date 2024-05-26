using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.MonthlyPurchaseDeclaration;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.MonthlyPurchaseDeclarationSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MonthlyPurchaseDeclarationFeature.Queries
{
    public class GetMonthlyPurchaseDeclarationByIdQuery : IRequest<Response<MonthlyPurchaseDeclarationDto>>
    {
        public int Id { get; set; }
    }
    public class GetMonthlyPurchaseDeclarationByIdQueryHandler : IRequestHandler<GetMonthlyPurchaseDeclarationByIdQuery, Response<MonthlyPurchaseDeclarationDto>>
    {
        private readonly IRepositoryAsync<MonthlyPurchaseDeclaration> _repositoryAsync;
        private readonly IMapper _mapper;
        public GetMonthlyPurchaseDeclarationByIdQueryHandler(IRepositoryAsync<MonthlyPurchaseDeclaration> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<MonthlyPurchaseDeclarationDto>> Handle(GetMonthlyPurchaseDeclarationByIdQuery request, CancellationToken cancellationToken)
        {
            var getQuotation = await _repositoryAsync.FirstOrDefaultAsync(new GetMonthlyPurchaseDeclarationByIdSpecification(request.Id));
            if (getQuotation == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<MonthlyPurchaseDeclarationDto>(getQuotation);
            return new Response<MonthlyPurchaseDeclarationDto>(dto);

        }
    }
}
