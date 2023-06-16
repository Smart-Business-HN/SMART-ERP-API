using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.DocumentTypeSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DocumentTypeFeature.Commands.UpdateDocumentTypeCommand
{
    public class UpdateDocumentTypeCommand : IRequest<Response<DocumentTypeDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class UpdateDocumentTypeCommandHandler : IRequestHandler<UpdateDocumentTypeCommand, Response<DocumentTypeDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<DocumentType> _repositoryAsync;
        private readonly IJwtService _jwtService;

        public UpdateDocumentTypeCommandHandler(IMapper mapper, IRepositoryAsync<DocumentType> repositoryAsync, IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
        }
        public async Task<Response<DocumentTypeDto>> Handle(UpdateDocumentTypeCommand request, CancellationToken cancellationToken)
        {
            var documentType = await _repositoryAsync.GetByIdAsync(request.Id);
            if (documentType == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var checkIfExist = await _repositoryAsync.FirstOrDefaultAsync(
                    new FilterDocumentTypeSpecification(request.Name, request.Id));
            if (checkIfExist != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            documentType.Name = request.Name;
            documentType.ModificationDate = DateTime.Now;
            documentType.ModificatedBy = _jwtService.GetSubjectToken();
            documentType.IsActive = request.IsActive;
            await _repositoryAsync.UpdateAsync(documentType);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<DocumentTypeDto>(documentType);
            return new Response<DocumentTypeDto>(dto, message: $"{request.Name} actualizado exitosamente");
        }
    }
}
