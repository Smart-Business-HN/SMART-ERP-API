using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DataSheetFeature.Queries
{
    public class GetDataSheetByIdQuery : IRequest<Response<DataSheetDto>>
    {
        public int Id { get; set; }
    }

    public class GetDataSheetByIdQueryHandler : IRequestHandler<GetDataSheetByIdQuery, Response<DataSheetDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<DataSheet> _repositoryAsync;

        public GetDataSheetByIdQueryHandler(IMapper mapper, IRepositoryAsync<DataSheet> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<DataSheetDto>> Handle(GetDataSheetByIdQuery request, CancellationToken cancellationToken)
        {
            var dataSheet = await _repositoryAsync.GetByIdAsync(request.Id);
            if (dataSheet == null)
            {

            }
            var dto = _mapper.Map<DataSheetDto>(dataSheet);
            return new Response<DataSheetDto>(dto);
        }
    }
}
