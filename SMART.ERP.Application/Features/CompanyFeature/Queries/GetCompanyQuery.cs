using AutoMapper;
using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CompanySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Company;

namespace SMART.ERP.Application.Features.CompanyFeature.Queries
{
    public class GetCompanyQuery : IRequest<Response<CompanyDto>>
    {
        public class GetCompanyQueryHandler : IRequestHandler<GetCompanyQuery, Response<CompanyDto>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<Company> _repositoryAsync;

            public GetCompanyQueryHandler(IMapper mapper, IRepositoryAsync<Company> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<Response<CompanyDto>> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
            {
                var company = await _repositoryAsync.ListAsync(new CompanyIncludesSpecification());
                if (company == null)
                {
                    throw new KeyNotFoundException($"No se encontro la compañia");
                }
                var dto = _mapper.Map<CompanyDto>(company[0]);
                return new Response<CompanyDto>(dto);
            }
        }
    }
}
