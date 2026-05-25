using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.FiscalPeriod;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.FiscalPeriodSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.FiscalPeriodFeature.Queries
{
    public class GetAllFiscalYearsQuery : IRequest<Response<List<FiscalYearDto>>>
    {
        public class GetAllFiscalYearsQueryHandler : IRequestHandler<GetAllFiscalYearsQuery, Response<List<FiscalYearDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<FiscalYear> _repositoryAsync;

            public GetAllFiscalYearsQueryHandler(IMapper mapper, IRepositoryAsync<FiscalYear> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<Response<List<FiscalYearDto>>> Handle(GetAllFiscalYearsQuery request, CancellationToken cancellationToken)
            {
                var years = await _repositoryAsync.ListAsync(new AllFiscalYearsWithPeriodsSpecification(), cancellationToken);
                var dto = _mapper.Map<List<FiscalYearDto>>(years);
                return new Response<List<FiscalYearDto>>(dto);
            }
        }
    }
}
