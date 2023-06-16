using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.LossReasonSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.LossReasonFeature.Queries
{
    public class GetAllLossReasonsQuery : IRequest<PagedResponse<List<LossReasonDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllLossReasonsQueryHandler : IRequestHandler<GetAllLossReasonsQuery, PagedResponse<List<LossReasonDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<LossReason> _repositoryAsync;

            public GetAllLossReasonsQueryHandler(IMapper mapper, IRepositoryAsync<LossReason> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<LossReasonDto>>> Handle(GetAllLossReasonsQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var lossReasons = await _repositoryAsync.ListAsync(
                    new PaginationLossReasonSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<LossReasonDto>>(lossReasons);
                return new PagedResponse<List<LossReasonDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
