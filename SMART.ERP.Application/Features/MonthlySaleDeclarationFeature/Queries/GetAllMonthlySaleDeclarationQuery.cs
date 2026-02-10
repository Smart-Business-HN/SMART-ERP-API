using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.MonthlySaleDeclaration;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.MonthlySaleDeclarationSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MonthlySaleDeclarationFeature.Queries
{
    public class GetAllMonthlySaleDeclarationQuery : IRequest<PagedResponse<List<MonthlySaleDeclarationDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }
    public class GetAllMonthlySaleDeclarationQueryHandler : IRequestHandler<GetAllMonthlySaleDeclarationQuery, PagedResponse<List<MonthlySaleDeclarationDto>>>
    {
        private readonly IRepositoryAsync<MonthlySaleDeclaration> _repositoryAsync;
        private readonly IMapper _mapper;
        public GetAllMonthlySaleDeclarationQueryHandler(IRepositoryAsync<MonthlySaleDeclaration> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<PagedResponse<List<MonthlySaleDeclarationDto>>> Handle(GetAllMonthlySaleDeclarationQuery request, CancellationToken cancellationToken)
        {
            if (request.All)
            {
                request.PageNumber = 0;
                request.PageSize = await _repositoryAsync.CountAsync();
            }
            var declarations = await _repositoryAsync.ListAsync(new GetAllMonthlySaleDeclarationSpecification(request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column));
            var dto = _mapper.Map<List<MonthlySaleDeclarationDto>>(declarations);
            return new PagedResponse<List<MonthlySaleDeclarationDto>>(dto, request.PageNumber, request.PageSize, request.All ? request.PageSize : await _repositoryAsync.CountAsync());
        }
    }
}
