using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.CountryFeature.Commands.DeleteCountryCommand
{
    public class DeleteCountryCommand : IRequest<Response<Country>>
    {
        public int Id { get; set; }
    }

    public class DeleteCountryCommandHandler : IRequestHandler<DeleteCountryCommand, Response<Country>>
    {
        private readonly IRepositoryAsync<Country> _repositoryAsync;

        public DeleteCountryCommandHandler(IRepositoryAsync<Country> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<Country>> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
            var checkIfExits = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkIfExits == null)
                throw new ApiException($"No se encontro ningun registro con el id: {request.Id}");

            await _repositoryAsync.DeleteAsync(checkIfExits);
            await _repositoryAsync.SaveChangesAsync();

            return new Response<Country>("Pais eliminado correctamente");
        }
    }
}
