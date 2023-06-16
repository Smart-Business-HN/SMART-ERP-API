using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.DataSheetSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DataSheetFeature.Queries
{
    public class GetAllDataSheetsQuery : IRequest<PagedResponse<List<DataSheetDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }

        public class GetAllDataSheetsQueryHandler : IRequestHandler<GetAllDataSheetsQuery, PagedResponse<List<DataSheetDto>>>
        {
            private readonly IMapper _mapper;
            private readonly IRepositoryAsync<DataSheet> _repositoryAsync;

            public GetAllDataSheetsQueryHandler(IMapper mapper, IRepositoryAsync<DataSheet> repositoryAsync)
            {
                _mapper = mapper;
                _repositoryAsync = repositoryAsync;
            }
            public async Task<PagedResponse<List<DataSheetDto>>> Handle(GetAllDataSheetsQuery request, CancellationToken cancellationToken)
            {
                if (request.All)
                {
                    request.PageNumber = 0;
                    request.PageSize = await _repositoryAsync.CountAsync();
                }

                var dataSheets = await _repositoryAsync.ListAsync(
                    new FilterAndPaginationDataSheetSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
                var dto = _mapper.Map<List<DataSheetDto>>(dataSheets);
                return new PagedResponse<List<DataSheetDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
            }
        }
    }
}
