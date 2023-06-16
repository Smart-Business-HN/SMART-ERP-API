using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Rootcloud;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.MachinerySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MachineryFailureFeature.Queries
{
    public class GetAllMachineryFailuresQuery : IRequest<PagedResponse<List<MachineryFailureDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }

    public class GetAllMachineryFailuresQueryHandler : IRequestHandler<GetAllMachineryFailuresQuery, PagedResponse<List<MachineryFailureDto>>>
    {
        private readonly IRepositoryAsync<MachineryFailure> _repositoryAsync;
        private readonly IMapper _mapper;

        public GetAllMachineryFailuresQueryHandler(IRepositoryAsync<MachineryFailure> repositoryAsync,
            IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<PagedResponse<List<MachineryFailureDto>>> Handle(GetAllMachineryFailuresQuery request, CancellationToken cancellationToken)
        {
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = await _repositoryAsync.CountAsync();
            }

            var lossReasons = await _repositoryAsync.ListAsync(
                new PaginationFailureSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
            var dto = _mapper.Map<List<MachineryFailureDto>>(lossReasons);
            return new PagedResponse<List<MachineryFailureDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
        }
    }
}
