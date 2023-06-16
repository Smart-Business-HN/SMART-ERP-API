using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CityFeature.Commands.DeleteCityCommand
{
    public class DeleteCityCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteCityCommandHandler : IRequestHandler<DeleteCityCommand, Response<string>>
    {
        private readonly IRepositoryAsync<City> _repositoryAsync;

        public DeleteCityCommandHandler(IRepositoryAsync<City> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteCityCommand request, CancellationToken cancellationToken)
        {
            var checkCity = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkCity == null)
            {
                throw new KeyNotFoundException($"No se encontro la ciudad con id {request.Id}");
            }

            try
            {
                await _repositoryAsync.DeleteAsync(checkCity);
                await _repositoryAsync.SaveChangesAsync();

                return new Response<string>("Eliminado correctamente");
            }
            catch (Exception)
            {
                throw new ApiException("Ocurrio un error al tratar de eliminar este registro, verifica que no se este utilizando en otro registro.");
            }
        }
    }
}
