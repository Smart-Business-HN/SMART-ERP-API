using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.AssociatedCompany;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.AssociatedCompanySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.AssociatedCompanyFeature.Commands.UpdateAssociatedCompanyCommand;

public class UpdateAssociatedCompanyCommand : IRequest<Response<AssociatedCompanyDto>>
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? RTN { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateAssociatedCompanyCommandHandler : IRequestHandler<UpdateAssociatedCompanyCommand, Response<AssociatedCompanyDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryAsync<AssociatedCompany> _repositoryAsync;

    public UpdateAssociatedCompanyCommandHandler(IMapper mapper, IRepositoryAsync<AssociatedCompany> repositoryAsync)
    {
        _mapper = mapper;
        _repositoryAsync = repositoryAsync;
    }

    public async Task<Response<AssociatedCompanyDto>> Handle(UpdateAssociatedCompanyCommand request, CancellationToken cancellationToken)
    {
        var record = await _repositoryAsync.GetByIdAsync(request.Id, cancellationToken);
        if (record == null)
        {
            throw new KeyNotFoundException($"No se encontró ningún registro con el id {request.Id}");
        }

        var checkIfExist = await _repositoryAsync.FirstOrDefaultAsync(
            new FilterAssociatedCompanySpecification(request.Name, record.EcommerceUserId, request.Id), cancellationToken);
        if (checkIfExist != null)
        {
            throw new ApiException($"Ya existe una sociedad con el nombre {request.Name}");
        }

        record.Name = request.Name;
        record.RTN = request.RTN;
        record.PhoneNumber = request.PhoneNumber;
        record.Address = request.Address;
        record.Email = request.Email;
        record.IsActive = request.IsActive;

        await _repositoryAsync.UpdateAsync(record, cancellationToken);
        await _repositoryAsync.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<AssociatedCompanyDto>(record);
        return new Response<AssociatedCompanyDto>(dto, message: $"{record.Name} actualizado correctamente");
    }
}
