using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.DTOs.Status;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Features.StatusFeature.Commands.CreateStatusCommand;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.StatusSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.PrefixFeature.Command.CreatePrefixCommand
{
    public class CreatePrefixCommand : IRequest<Response<PrefixDto>>
    {
        public string Format { get; set; } = null!;
        public int InternalDocumentId { get; set; }
        public bool ItIsTaken { get; set; }
    }
    public class CreatePrefixCommandHandler : IRequestHandler<CreatePrefixCommand, Response<PrefixDto>>
    {
        private readonly IRepositoryAsync<Prefix> _repositoryAsync;
        private readonly IRepositoryAsync<InternalDocument> _internalDocumentRepositoryAsync;
        private readonly IMapper _mapper;

        public CreatePrefixCommandHandler(IRepositoryAsync<Prefix> repositoryAsync, IRepositoryAsync<InternalDocument> internalDocumentRepositoryAsync
            , IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _internalDocumentRepositoryAsync = internalDocumentRepositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<PrefixDto>> Handle(CreatePrefixCommand request, CancellationToken cancellationToken)
        {

            var checkInternalDocument = await _internalDocumentRepositoryAsync.GetByIdAsync(request.InternalDocumentId);
            if (checkInternalDocument == null)
            {
                throw new KeyNotFoundException($"No se encontro el documento interno con id {request.InternalDocumentId}");
            }

            var newRecord = _mapper.Map<Prefix>(request);
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<PrefixDto>(data);
            return new Response<PrefixDto>(dto, "Agregado correctamente");
        }
    }
}
