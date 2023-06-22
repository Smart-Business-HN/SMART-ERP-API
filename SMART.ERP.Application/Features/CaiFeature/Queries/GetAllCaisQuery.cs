using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Cai;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CaiSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.CaiFeature.Queries
{
    public class GetAllCaisQuery : IRequest<PagedResponse<List<CaiDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }
    public class GetAllCaisQueryHandler : IRequestHandler<GetAllCaisQuery, PagedResponse<List<CaiDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Cai> _repositoryAsync;
        public GetAllCaisQueryHandler(IMapper mapper, IRepositoryAsync<Cai> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }
        public async Task<PagedResponse<List<CaiDto>>> Handle(GetAllCaisQuery request, CancellationToken cancellationToken)
        {
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = await _repositoryAsync.CountAsync();
            }
            var cais = await _repositoryAsync.ListAsync(new FilterAndPaginationCaisSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
            var dto = _mapper.Map<List<CaiDto>>(cais);
            return new PagedResponse<List<CaiDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
        }
    }
}
