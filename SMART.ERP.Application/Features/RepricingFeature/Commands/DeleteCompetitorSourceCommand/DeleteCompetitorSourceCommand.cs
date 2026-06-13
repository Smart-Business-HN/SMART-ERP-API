using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.RepricingFeature.Commands.DeleteCompetitorSourceCommand
{
    public class DeleteCompetitorSourceCommand : IRequest<Response<bool>>
    {
        public int Id { get; set; }

        public class Handler : IRequestHandler<DeleteCompetitorSourceCommand, Response<bool>>
        {
            private readonly IRepositoryAsync<CompetitorSource> _repo;

            public Handler(IRepositoryAsync<CompetitorSource> repo) => _repo = repo;

            public async Task<Response<bool>> Handle(DeleteCompetitorSourceCommand request, CancellationToken ct)
            {
                var entity = await _repo.GetByIdAsync(request.Id, ct)
                    ?? throw new KeyNotFoundException($"Fuente de competencia {request.Id} no encontrada");

                await _repo.DeleteAsync(entity, ct);
                await _repo.SaveChangesAsync(ct);
                return new Response<bool>(true, "Fuente de competencia eliminada");
            }
        }
    }
}
