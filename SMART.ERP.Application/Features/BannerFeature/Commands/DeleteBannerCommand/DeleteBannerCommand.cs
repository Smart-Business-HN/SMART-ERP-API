using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.BannerFeature.Commands.DeleteBannerCommand
{
    public class DeleteBannerCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeleteBannerCommandHandler : IRequestHandler<DeleteBannerCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Banner> _repositoryAsync;

        public DeleteBannerCommandHandler(IRepositoryAsync<Banner> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(DeleteBannerCommand request, CancellationToken cancellationToken)
        {
            var banner = await _repositoryAsync.GetByIdAsync(request.Id);
            if (banner == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            await _repositoryAsync.DeleteAsync(banner);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>($"{banner.FileName} eliminado correctamente", "Eliminado correctamente");
        }
    }
}
