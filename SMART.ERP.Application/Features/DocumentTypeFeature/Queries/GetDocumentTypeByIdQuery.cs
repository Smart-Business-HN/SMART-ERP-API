using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DocumentTypeFeature.Queries
{
    public class GetDocumentTypeByIdQuery : IRequest<Response<DocumentTypeDto>>
    {
        public int Id { get; set; }
    }

    public class GetDocumentTypeByIdQueryHandler : IRequestHandler<GetDocumentTypeByIdQuery, Response<DocumentTypeDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<DocumentType> _repositoryAsync;

        public GetDocumentTypeByIdQueryHandler(IMapper mapper, IRepositoryAsync<DocumentType> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<DocumentTypeDto>> Handle(GetDocumentTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var documentType = await _repositoryAsync.GetByIdAsync(request.Id);
            if (documentType == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
            }
            var dto = _mapper.Map<DocumentTypeDto>(documentType);
            return new Response<DocumentTypeDto>(dto);
        }
    }
}
