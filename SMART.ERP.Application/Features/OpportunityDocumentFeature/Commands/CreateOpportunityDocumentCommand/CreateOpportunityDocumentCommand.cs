using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityDocumentFeature.Commands.CreateOpportunityDocumentCommand
{
    public class CreateOpportunityDocumentCommand : IRequest<Response<OpportunityDocumentDto>>
    {
        public string Name { get; set; } = null!;
        public string Url { get; set; } = null!;
        public Guid UserId { get; set; }
        public int DocumentTypeId { get; set; }
        public int OpportunityId { get; set; }
    }

    public class CreateOpportunityDocumentCommandHandler : IRequestHandler<CreateOpportunityDocumentCommand, Response<OpportunityDocumentDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<OpportunityDocument> _repositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<DocumentType> _documentTypeRepositoryAsync;
        private readonly IJwtService _jwtService;

        public CreateOpportunityDocumentCommandHandler(
            IMapper mapper,
            IRepositoryAsync<OpportunityDocument> repositoryAsync,
            IRepositoryAsync<User> userRepositoryAsync,
            IRepositoryAsync<DocumentType> documentTypeRepositoryAsync,
            IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _documentTypeRepositoryAsync = documentTypeRepositoryAsync;
            _jwtService = jwtService;
        }

        public async Task<Response<OpportunityDocumentDto>> Handle(CreateOpportunityDocumentCommand request, CancellationToken cancellationToken)
        {
            var getUser = await _userRepositoryAsync.GetByIdAsync(request.UserId);
            if (getUser == null)
                throw new ApiException($"No se encontro el usuario con el id: {request.UserId}");

            var getDocumentType = await _documentTypeRepositoryAsync.GetByIdAsync(request.DocumentTypeId);
            if (getDocumentType == null)
                throw new ApiException($"No se encontro el tipo de documento con el id: {request.DocumentTypeId}");

            var newRecord = _mapper.Map<OpportunityDocument>(request);
            newRecord.CreationDate = DateTime.Now;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            var response = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            newRecord.DocumentType = getDocumentType;
            newRecord.User = getUser;
            var dto = _mapper.Map<OpportunityDocumentDto>(response);
            return new Response<OpportunityDocumentDto>(dto);
        }
    }
}
