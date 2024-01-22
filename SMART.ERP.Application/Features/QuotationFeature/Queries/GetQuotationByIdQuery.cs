using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.QuotationSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.QuotationFeature.Queries
{
    public class GetQuotationByIdQuery : IRequest<Response<QuotationDto>>
    {
        public int Id { get; set; }
    }
    public class GetQuotationByIdQueryHandler : IRequestHandler<GetQuotationByIdQuery, Response<QuotationDto>>
    {
        private readonly IRepositoryAsync<Quotation> _repositoryAsync;
        private readonly IMapper _mapper;
        public GetQuotationByIdQueryHandler(IRepositoryAsync<Quotation> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<QuotationDto>> Handle(GetQuotationByIdQuery request, CancellationToken cancellationToken)
        {
            var getQuotation = await _repositoryAsync.FirstOrDefaultAsync(new FilterQuotationByIdSpecification(request.Id));
            if (getQuotation == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<QuotationDto>(getQuotation);
            return new Response<QuotationDto>(dto);
        }
    }
}
