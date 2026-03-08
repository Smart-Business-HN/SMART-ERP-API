using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.QuotationSnapshotSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.QuotationFeature.Queries
{
    public class GetQuotationSnapshotByIdQuery : IRequest<Response<QuotationSnapshotDetailDto>>
    {
        public int Id { get; set; }
    }
    public class GetQuotationSnapshotByIdQueryHandler : IRequestHandler<GetQuotationSnapshotByIdQuery, Response<QuotationSnapshotDetailDto>>
    {
        private readonly IRepositoryAsync<QuotationSnapshot> _repositoryAsync;
        private readonly IMapper _mapper;
        public GetQuotationSnapshotByIdQueryHandler(IRepositoryAsync<QuotationSnapshot> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<QuotationSnapshotDetailDto>> Handle(GetQuotationSnapshotByIdQuery request, CancellationToken cancellationToken)
        {
            var snapshot = await _repositoryAsync.FirstOrDefaultAsync(new GetQuotationSnapshotByIdSpecification(request.Id));
            if (snapshot == null)
            {
                throw new ApiException($"No existe un snapshot con el Id {request.Id}");
            }
            var dto = _mapper.Map<QuotationSnapshotDetailDto>(snapshot);
            dto.SnapshotData = JsonConvert.DeserializeObject<QuotationSnapshotDataDto>(snapshot.SnapshotData);
            return new Response<QuotationSnapshotDetailDto>(dto);
        }
    }
}
