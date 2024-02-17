using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MajorIncomeAccountFeature.Commands.DeleteMajorIncomeAccountCommand
{
    public class DeleteMajorIncomeAccountCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }
    public class DeleteMajorIncomeAccountCommandHandler : IRequestHandler<DeleteMajorIncomeAccountCommand, Response<string>>
    {
        private readonly IRepositoryAsync<MajorIncomeAccount> _repositoryAsync;
        public DeleteMajorIncomeAccountCommandHandler(IRepositoryAsync<MajorIncomeAccount> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<string>> Handle(DeleteMajorIncomeAccountCommand request, CancellationToken cancellationToken)
        {
            var checkAccount = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkAccount == null)
            {
                throw new KeyNotFoundException($"No se encontro una cuenta con id {request.Id}");
            }
            try
            {
                await _repositoryAsync.DeleteAsync(checkAccount);
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
