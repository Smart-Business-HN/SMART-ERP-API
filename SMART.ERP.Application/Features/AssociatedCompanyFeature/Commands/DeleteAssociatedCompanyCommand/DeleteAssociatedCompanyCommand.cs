using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.AssociatedCompanyFeature.Commands.DeleteAssociatedCompanyCommand;

public class DeleteAssociatedCompanyCommand : IRequest<Response<string>>
{
    public int Id { get; set; }
}

public class DeleteAssociatedCompanyCommandHandler : IRequestHandler<DeleteAssociatedCompanyCommand, Response<string>>
{
    private readonly IRepositoryAsync<AssociatedCompany> _repositoryAsync;

    public DeleteAssociatedCompanyCommandHandler(IRepositoryAsync<AssociatedCompany> repositoryAsync)
    {
        _repositoryAsync = repositoryAsync;
    }

    public async Task<Response<string>> Handle(DeleteAssociatedCompanyCommand request, CancellationToken cancellationToken)
    {
        var record = await _repositoryAsync.GetByIdAsync(request.Id, cancellationToken);
        if (record == null)
        {
            throw new KeyNotFoundException($"No se encontró ningún registro con el id {request.Id}");
        }

        await _repositoryAsync.DeleteAsync(record, cancellationToken);
        await _repositoryAsync.SaveChangesAsync(cancellationToken);

        return new Response<string>($"{record.Name} eliminado correctamente", "Eliminado correctamente");
    }
}
