using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.EcommerceUser;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.EcommerceUserFeature.Queries.GetEcommerceUserByIdQuery;

public class GetEcommerceUserByIdQuery : IRequest<Response<EcommerceUserDto>>
{
    public Guid Id { get; set; }
}
public class GetEcommerceUserByIdQueryHandler : IRequestHandler<GetEcommerceUserByIdQuery, Response<EcommerceUserDto>>
{
    private readonly IRepositoryAsync<EcommerceUser> _repositoryAsync;
    private IMapper _mapper;

    public GetEcommerceUserByIdQueryHandler(IRepositoryAsync<EcommerceUser> repositoryAsync, IMapper mapper)
    {
        _repositoryAsync = repositoryAsync;
        _mapper = mapper;
    }
    public async Task<Response<EcommerceUserDto>> Handle(GetEcommerceUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _repositoryAsync.GetByIdAsync(request.Id);
        if (user == null)
        {
            throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
        }
        var dto = _mapper.Map<EcommerceUserDto>(user);
        return new Response<EcommerceUserDto>(dto);
    }
}