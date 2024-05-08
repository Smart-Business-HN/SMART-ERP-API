using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.DailyClose;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.DailyCloseSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DailyClosinFeature.Queries
{
    public class GetAllDailyClosingQuery : IRequest<PagedResponse<List<DailyCloseDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }
    public class GetAllDailyClosingQueryHandler : IRequestHandler<GetAllDailyClosingQuery, PagedResponse<List<DailyCloseDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<DailyClose> _repositoryAsync;
        public GetAllDailyClosingQueryHandler(IMapper mapper, IRepositoryAsync<DailyClose> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }
        public async Task<PagedResponse<List<DailyCloseDto>>> Handle(GetAllDailyClosingQuery request, CancellationToken cancellationToken)
        {
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = await _repositoryAsync.CountAsync();
            }
            var dailyClosings = await _repositoryAsync.ListAsync(new FilterAndPaginationDailyClosingSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
            var dto = _mapper.Map<List<DailyCloseDto>>(dailyClosings);
            return new PagedResponse<List<DailyCloseDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
        }
    }
}
