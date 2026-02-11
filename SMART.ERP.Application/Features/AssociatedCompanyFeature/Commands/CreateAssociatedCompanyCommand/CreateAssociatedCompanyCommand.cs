using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.AssociatedCompany;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.AssociatedCompanySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.AssociatedCompanyFeature.Commands.CreateAssociatedCompanyCommand;

public class CreateAssociatedCompanyCommand : IRequest<Response<AssociatedCompanyDto>>
{
    public Guid EcommerceUserId { get; set; }
    public string Name { get; set; } = null!;
    public string? RTN { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? Email { get; set; }
}

public class CreateAssociatedCompanyCommandHandler : IRequestHandler<CreateAssociatedCompanyCommand, Response<AssociatedCompanyDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryAsync<AssociatedCompany> _repositoryAsync;

    public CreateAssociatedCompanyCommandHandler(IMapper mapper, IRepositoryAsync<AssociatedCompany> repositoryAsync)
    {
        _mapper = mapper;
        _repositoryAsync = repositoryAsync;
    }

    public async Task<Response<AssociatedCompanyDto>> Handle(CreateAssociatedCompanyCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repositoryAsync.FirstOrDefaultAsync(
            new FilterAssociatedCompanySpecification(request.Name, request.EcommerceUserId, null), cancellationToken);
        if (existing != null)
        {
            throw new ApiException($"Ya existe una sociedad con el nombre {request.Name}");
        }

        var newRecord = _mapper.Map<AssociatedCompany>(request);
        newRecord.CreationDate = DateTime.UtcNow;
        newRecord.IsActive = true;

        var data = await _repositoryAsync.AddAsync(newRecord, cancellationToken);
        await _repositoryAsync.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<AssociatedCompanyDto>(data);
        return new Response<AssociatedCompanyDto>(dto, message: $"{request.Name} creado exitosamente");
    }
}
