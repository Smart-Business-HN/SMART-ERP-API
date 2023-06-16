using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.DocumentTypeSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DocumentTypeFeature.Commands.CreateDocumentTypeCommand
{
    public class CreateDocumentTypeCommand : IRequest<Response<DocumentTypeDto>>
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class CreateDocumentTypeCommandHandler : IRequestHandler<CreateDocumentTypeCommand, Response<DocumentTypeDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<DocumentType> _repositoryAsync;
        private readonly IJwtService _jwtService;

        public CreateDocumentTypeCommandHandler(IMapper mapper, IRepositoryAsync<DocumentType> repositoryAsync, IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
        }
        public async Task<Response<DocumentTypeDto>> Handle(CreateDocumentTypeCommand request, CancellationToken cancellationToken)
        {
            var documentType = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterDocumentTypeSpecification(request.Name, null));
            if (documentType != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            var newRecord = _mapper.Map<DocumentType>(request);
            newRecord.CreationDate = DateTime.Now;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<DocumentTypeDto>(data);
            return new Response<DocumentTypeDto>(dto, message: $"{request.Name} creado exitosamente");
        }
    }
}
