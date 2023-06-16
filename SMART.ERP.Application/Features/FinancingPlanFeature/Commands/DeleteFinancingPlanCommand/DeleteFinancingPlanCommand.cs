using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.FinancingPlanFeature.Commands.DeleteFinancingPlanCommand
{
    public class DeleteFinancingPlanCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteFinancingPlanCommandHandler : IRequestHandler<DeleteFinancingPlanCommand, Response<string>>
    {
        private readonly IRepositoryAsync<FinancingPlan> _repositoryAsync;

        public DeleteFinancingPlanCommandHandler(IRepositoryAsync<FinancingPlan> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<string>> Handle(DeleteFinancingPlanCommand request, CancellationToken cancellationToken)
        {
            var financingPlan = await _repositoryAsync.GetByIdAsync(request.Id);
            if (financingPlan == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(financingPlan);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{financingPlan.Name} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
