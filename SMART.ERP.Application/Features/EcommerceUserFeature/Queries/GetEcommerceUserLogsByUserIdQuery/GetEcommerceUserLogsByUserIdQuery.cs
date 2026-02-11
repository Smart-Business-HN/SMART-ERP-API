using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.LogEcommerceUser;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.LogEcommerceUserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.EcommerceUserFeature.Queries.GetEcommerceUserLogsByUserIdQuery;

public class GetEcommerceUserLogsByUserIdQuery : IRequest<PagedResponse<List<LogEcommerceUserDto>>>
{
    public Guid EcommerceUserId { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int? ActionType { get; set; }
}

public class GetEcommerceUserLogsByUserIdQueryHandler
    : IRequestHandler<GetEcommerceUserLogsByUserIdQuery, PagedResponse<List<LogEcommerceUserDto>>>
{
    private readonly IRepositoryAsync<LogEcommerceUser> _repositoryAsync;
    private readonly IMapper _mapper;

    public GetEcommerceUserLogsByUserIdQueryHandler(
        IRepositoryAsync<LogEcommerceUser> repositoryAsync,
        IMapper mapper)
    {
        _repositoryAsync = repositoryAsync;
        _mapper = mapper;
    }

    public async Task<PagedResponse<List<LogEcommerceUserDto>>> Handle(
        GetEcommerceUserLogsByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        var logs = await _repositoryAsync.ListAsync(
            new FilterLogEcommerceUserSpecification(
                request.EcommerceUserId,
                request.ActionType,
                request.PageNumber,
                request.PageSize),
            cancellationToken);

        var totalCount = await _repositoryAsync.CountAsync(
            new CountLogEcommerceUserSpecification(
                request.EcommerceUserId,
                request.ActionType),
            cancellationToken);

        var dto = _mapper.Map<List<LogEcommerceUserDto>>(logs);
        return new PagedResponse<List<LogEcommerceUserDto>>(dto, request.PageNumber, request.PageSize, totalCount);
    }
}
