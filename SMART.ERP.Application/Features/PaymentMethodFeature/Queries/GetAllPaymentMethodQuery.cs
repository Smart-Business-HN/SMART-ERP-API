using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.PaymentMethod;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.PaymentMethodSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.PaymentMethodFeature.Queries;

public class GetAllPaymentMethodQuery : IRequest<PagedResponse<List<PaymentMethodDto>>>
{
    public Guid EcommerceUserId { get; set; }
    public string? Parameter { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? Order { get; set; }
    public string? Column { get; set; }
    public bool All { get; set; }
}

public class GetAllPaymentMethodQueryHandler : IRequestHandler<GetAllPaymentMethodQuery, PagedResponse<List<PaymentMethodDto>>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryAsync<PaymentMethod> _repositoryAsync;

    public GetAllPaymentMethodQueryHandler(IMapper mapper, IRepositoryAsync<PaymentMethod> repositoryAsync)
    {
        _mapper = mapper;
        _repositoryAsync = repositoryAsync;
    }

    public async Task<PagedResponse<List<PaymentMethodDto>>> Handle(GetAllPaymentMethodQuery request, CancellationToken cancellationToken)
    {
        if (request.All)
        {
            request.PageNumber = 0;
            request.PageSize = await _repositoryAsync.CountAsync(cancellationToken);
        }

        var records = await _repositoryAsync.ListAsync(
            new FilterAndPaginationPaymentMethodSpecification(
                request.EcommerceUserId, request.Parameter, request.PageNumber, request.PageSize, request.Order, request.Column),
            cancellationToken);

        var dto = _mapper.Map<List<PaymentMethodDto>>(records);
        return new PagedResponse<List<PaymentMethodDto>>(dto, request.PageNumber, request.PageSize,
            request.All ? request.PageSize : await _repositoryAsync.CountAsync(cancellationToken));
    }
}
