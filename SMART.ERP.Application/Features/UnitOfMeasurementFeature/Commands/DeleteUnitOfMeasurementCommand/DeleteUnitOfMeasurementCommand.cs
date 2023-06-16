using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.UnitOfMeasurementFeature.Commands.DeleteUnitOfMeasurementCommand
{
    public class DeleteUnitOfMeasurementCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteUnitOfMeasurementCommandHandler : IRequestHandler<DeleteUnitOfMeasurementCommand, Response<string>>
    {
        private readonly IRepositoryAsync<UnitOfMeasurement> _repositoryAsync;

        public DeleteUnitOfMeasurementCommandHandler(IRepositoryAsync<UnitOfMeasurement> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<string>> Handle(DeleteUnitOfMeasurementCommand request, CancellationToken cancellationToken)
        {
            var unitOfMeasurement = await _repositoryAsync.GetByIdAsync(request.Id);
            if (unitOfMeasurement == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(unitOfMeasurement);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{unitOfMeasurement.Name} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
