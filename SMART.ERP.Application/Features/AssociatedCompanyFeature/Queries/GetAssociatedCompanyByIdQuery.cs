using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.AssociatedCompany;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.AssociatedCompanyFeature.Queries;

public class GetAssociatedCompanyByIdQuery : IRequest<Response<AssociatedCompanyDto>>
{
    public int Id { get; set; }
}

public class GetAssociatedCompanyByIdQueryHandler : IRequestHandler<GetAssociatedCompanyByIdQuery, Response<AssociatedCompanyDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryAsync<AssociatedCompany> _repositoryAsync;

    public GetAssociatedCompanyByIdQueryHandler(IMapper mapper, IRepositoryAsync<AssociatedCompany> repositoryAsync)
    {
        _mapper = mapper;
        _repositoryAsync = repositoryAsync;
    }

    public async Task<Response<AssociatedCompanyDto>> Handle(GetAssociatedCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var record = await _repositoryAsync.GetByIdAsync(request.Id, cancellationToken);
        if (record == null)
        {
            throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
        }

        var dto = _mapper.Map<AssociatedCompanyDto>(record);
        return new Response<AssociatedCompanyDto>(dto);
    }
}
