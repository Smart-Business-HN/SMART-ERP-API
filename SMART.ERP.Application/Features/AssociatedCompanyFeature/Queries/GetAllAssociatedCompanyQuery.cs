using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.AssociatedCompany;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.AssociatedCompanySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.AssociatedCompanyFeature.Queries;

public class GetAllAssociatedCompanyQuery : IRequest<PagedResponse<List<AssociatedCompanyDto>>>
{
    public Guid EcommerceUserId { get; set; }
    public string? Parameter { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? Order { get; set; }
    public string? Column { get; set; }
    public bool All { get; set; }
}

public class GetAllAssociatedCompanyQueryHandler : IRequestHandler<GetAllAssociatedCompanyQuery, PagedResponse<List<AssociatedCompanyDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryAsync<AssociatedCompany> _repositoryAsync;

    public GetAllAssociatedCompanyQueryHandler(IMapper mapper, IRepositoryAsync<AssociatedCompany> repositoryAsync)
    {
        _mapper = mapper;
        _repositoryAsync = repositoryAsync;
    }

    public async Task<PagedResponse<List<AssociatedCompanyDto>>> Handle(GetAllAssociatedCompanyQuery request, CancellationToken cancellationToken)
    {
        if (request.All)
        {
            request.PageNumber = 0;
            request.PageSize = await _repositoryAsync.CountAsync(cancellationToken);
        }

        var records = await _repositoryAsync.ListAsync(
            new FilterAndPaginationAssociatedCompanySpecification(
                request.EcommerceUserId, request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column),
            cancellationToken);

        var dto = _mapper.Map<List<AssociatedCompanyDto>>(records);
        return new PagedResponse<List<AssociatedCompanyDto>>(dto, request.PageNumber, request.PageSize,
            request.All ? request.PageSize : await _repositoryAsync.CountAsync(cancellationToken));
    }
}
