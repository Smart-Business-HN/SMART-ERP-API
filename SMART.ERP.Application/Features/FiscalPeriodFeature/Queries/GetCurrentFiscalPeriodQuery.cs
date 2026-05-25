using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.FiscalPeriod;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.FiscalPeriodSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.FiscalPeriodFeature.Queries
{
    /// <summary>Devuelve el período fiscal que contiene una fecha (por defecto, hoy).</summary>
    public class GetCurrentFiscalPeriodQuery : IRequest<Response<FiscalPeriodDto>>
    {
        public DateTime? Date { get; set; }

        public class GetCurrentFiscalPeriodQueryHandler : IRequestHandler<GetCurrentFiscalPeriodQuery, Response<FiscalPeriodDto>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<FiscalPeriod> _repositoryAsync;

            public GetCurrentFiscalPeriodQueryHandler(IMapper mapper, IRepositoryAsync<FiscalPeriod> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }

            public async Task<Response<FiscalPeriodDto>> Handle(GetCurrentFiscalPeriodQuery request, CancellationToken cancellationToken)
            {
                var date = request.Date ?? DateTime.Now;
                var period = await _repositoryAsync.FirstOrDefaultAsync(new FilterFiscalPeriodByDateSpecification(date), cancellationToken);
                if (period == null)
                    return new Response<FiscalPeriodDto>("No existe un período fiscal para la fecha indicada.");
                var dto = _mapper.Map<FiscalPeriodDto>(period);
                return new Response<FiscalPeriodDto>(dto);
            }
        }
    }
}
