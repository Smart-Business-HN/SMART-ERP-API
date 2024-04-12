using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.NonBillableExpenseFeature.Commands.DeleteNonBillableExpenseCommand
{
    public class DeleteNonBillableExpenseCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteNonBillableExpenseCommandHandler : IRequestHandler<DeleteNonBillableExpenseCommand, Response<string>>
    {
        private readonly IRepositoryAsync<NonBillableExpense> _repositoryAsync;
        private readonly IOutputCacheStore _outputCacheStored;

        public DeleteNonBillableExpenseCommandHandler(IRepositoryAsync<NonBillableExpense> repositoryAsync, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _outputCacheStored = outputCacheStored;
        }
        public async Task<Response<string>> Handle(DeleteNonBillableExpenseCommand request, CancellationToken cancellationToken)
        {
            var category = await _repositoryAsync.GetByIdAsync(request.Id);
            if (category == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(category);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_categories", cancellationToken);
            return new Response<string>($"Gasto no facturable eliminado correctamente", "Eliminado correctamente");
        }
    }
}
