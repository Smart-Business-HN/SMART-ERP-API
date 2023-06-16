using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.WinReasonSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.WinReasonFeature.Queries
{
    public class GetAllWinReasonsQuery : IRequest<PagedResponse<List<WinReasonDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
        public class GetAllWinReasonsQueryHandler : IRequestHandler<GetAllWinReasonsQuery, PagedResponse<List<WinReasonDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<WinReason> _repositoryAsync;

            public GetAllWinReasonsQueryHandler(IMapper mapper, IRepositoryAsync<WinReason> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<WinReasonDto>>> Handle(GetAllWinReasonsQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var winReasons = await _repositoryAsync.ListAsync(
                    new PaginationWinReasonSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<WinReasonDto>>(winReasons);
                return new PagedResponse<List<WinReasonDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
