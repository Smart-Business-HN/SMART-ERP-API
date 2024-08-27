using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Features.DepartmentFeature.Commands.DeleteDepartmentCommand;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DiscountFeature.Commands.DeleteDiscountCommand
{
    public class DeleteDiscountCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteDiscountCommandHandler : IRequestHandler<DeleteDiscountCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Discount> _repositoryAsync;

        public DeleteDiscountCommandHandler(IRepositoryAsync<Discount> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
        {
            var checkDepartment = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkDepartment == null)
            {
                throw new KeyNotFoundException($"No se encontro el descuento con id {request.Id}");
            }

            try
            {
                await _repositoryAsync.DeleteAsync(checkDepartment);
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