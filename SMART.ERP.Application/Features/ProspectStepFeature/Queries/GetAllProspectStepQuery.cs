using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Prospect;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProspectStepFeature.Queries
{
    public class GetAllProspectStepQuery : IRequest<PagedResponse<List<ProspectStepDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }

    public class GetAllProspectQueryHandler : IRequestHandler<GetAllProspectStepQuery, PagedResponse<List<ProspectStepDto>>>
    {
        private readonly IRepositoryAsync<ProspectStep> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllProspectQueryHandler(IRepositoryAsync<ProspectStep> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<PagedResponse<List<ProspectStepDto>>> Handle(GetAllProspectStepQuery request, CancellationToken cancellationToken)
        {
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = await _repositoryAsync.CountAsync();
            }

            var prospectSteps = await _repositoryAsync.ListAsync();
            var response = prospectSteps.Skip(request.PageNumber * request.PageSize).Take(request.PageSize);
            var dto = _mapper.Map<List<ProspectStepDto>>(response);
            return new PagedResponse<List<ProspectStepDto>>(dto, request.PageNumber, request.PageSize, request.Parameter == null ? request.PageSize : prospectSteps.Count);
        }
    }
}
