using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Prospect;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.ProspectSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ReportFeature.Queries
{
    public class MasterProspectReportQuery : IRequest<PagedResponse<List<ProspectDto>>>
    {
        public Guid? UserId { get; set; }
        public int? ProspectStepId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool All { get; set; }
    }

    public class MasterProspectReportQueryHandler : IRequestHandler<MasterProspectReportQuery, PagedResponse<List<ProspectDto>>>
    {
        private readonly IRepositoryAsync<Prospect> _repositoryAsync;
        private readonly IMapper _mapper;

        public MasterProspectReportQueryHandler(IRepositoryAsync<Prospect> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<PagedResponse<List<ProspectDto>>> Handle(MasterProspectReportQuery request, CancellationToken cancellationToken)
        {
            var prospects = await _repositoryAsync.ListAsync(new MasterProspectSpecification(request.UserId, request.ProspectStepId));
            var dto = _mapper.Map<List<ProspectDto>>(prospects);
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = prospects.Count;
            }
            dto = dto.Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
            return new PagedResponse<List<ProspectDto>>(dto, request.PageNumber, request.PageSize, prospects.Count);
        }
    }
}
