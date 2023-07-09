using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PrefixSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.PrefixFeature.Queries
{
    public class GetPrefixByInternalDocumentIdQuery : IRequest<List<PrefixDto>>
    {
        public int InternalDocumentId { get; set; }
    }
    public class GetPrefixByInternalDocumentIdQueryHandler : IRequestHandler<GetPrefixByInternalDocumentIdQuery, List<PrefixDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Prefix> _repositoryAsync;
        public GetPrefixByInternalDocumentIdQueryHandler(IMapper mapper, IRepositoryAsync<Prefix> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }
        public async Task<List<PrefixDto>> Handle (GetPrefixByInternalDocumentIdQuery request, CancellationToken cancellationToken)
        {
            var prefixes = await _repositoryAsync.ListAsync(new PrefixByInternalDocumentIdSpecification(request.InternalDocumentId));
            var dto = _mapper.Map<List<PrefixDto>>(prefixes);
            return new List<PrefixDto>(dto);
        }

    }
}
