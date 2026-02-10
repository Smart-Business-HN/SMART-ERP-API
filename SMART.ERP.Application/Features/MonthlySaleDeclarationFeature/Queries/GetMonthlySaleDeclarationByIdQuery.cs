using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.MonthlySaleDeclaration;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.MonthlySaleDeclarationSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MonthlySaleDeclarationFeature.Queries
{
    public class GetMonthlySaleDeclarationByIdQuery : IRequest<Response<MonthlySaleDeclarationDto>>
    {
        public int Id { get; set; }
    }
    public class GetMonthlySaleDeclarationByIdQueryHandler : IRequestHandler<GetMonthlySaleDeclarationByIdQuery, Response<MonthlySaleDeclarationDto>>
    {
        private readonly IRepositoryAsync<MonthlySaleDeclaration> _repositoryAsync;
        private readonly IMapper _mapper;
        public GetMonthlySaleDeclarationByIdQueryHandler(IRepositoryAsync<MonthlySaleDeclaration> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<MonthlySaleDeclarationDto>> Handle(GetMonthlySaleDeclarationByIdQuery request, CancellationToken cancellationToken)
        {
            var declaration = await _repositoryAsync.FirstOrDefaultAsync(new GetMonthlySaleDeclarationByIdSpecification(request.Id));
            if (declaration == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<MonthlySaleDeclarationDto>(declaration);
            return new Response<MonthlySaleDeclarationDto>(dto);
        }
    }
}
