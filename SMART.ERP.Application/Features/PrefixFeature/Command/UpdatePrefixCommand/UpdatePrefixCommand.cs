using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PrefixFeature.Command.UpdatePrefixCommand
{
    public class UpdatePrefixCommand : IRequest<Response<PrefixDto>>
    {
        public int Id { get; set; }
        public string Format { get; set; } = null!;
        public int InternalDocumentId { get; set; }
        public bool ItIsTaken { get; set; }
    }
    public class UpdatePrefixCommandHandler : IRequestHandler<UpdatePrefixCommand, Response<PrefixDto>>
    {
        private readonly IRepositoryAsync<Prefix> _repositoryAsync;
        private readonly IRepositoryAsync<InternalDocument> _internalDocumentRepositoryAsync;
        private readonly IMapper _mapper;

        public UpdatePrefixCommandHandler(IRepositoryAsync<Prefix> repositoryAsync, IRepositoryAsync<InternalDocument> internalDocumentRepositoryAsync
            , IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _internalDocumentRepositoryAsync = internalDocumentRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<PrefixDto>> Handle(UpdatePrefixCommand request, CancellationToken cancellationToken)
        {

            var prefix = await _repositoryAsync.GetByIdAsync(request.Id);
            if (prefix == null)
            {
                throw new KeyNotFoundException($"No se encontro el prefijo con id {request.Id}");
            }
            var checkDocumentType = await _internalDocumentRepositoryAsync.GetByIdAsync(request.InternalDocumentId);
            if(checkDocumentType == null)
            {
                throw new KeyNotFoundException($"No se encontro el documento interno con id {request.InternalDocumentId}");
            }

            prefix.Format = request.Format;
            prefix.InternalDocumentId = request.InternalDocumentId;
            prefix.ItIsTaken= request.ItIsTaken;
            await _repositoryAsync.UpdateAsync(prefix);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<PrefixDto>(prefix);
            return new Response<PrefixDto>(dto, message: $"{dto.Format} actualizado correctamente");
        }
    }
}
