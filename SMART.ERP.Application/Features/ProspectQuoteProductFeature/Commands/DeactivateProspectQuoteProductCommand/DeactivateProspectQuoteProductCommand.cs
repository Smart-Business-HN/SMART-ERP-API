using AutoMapper;
using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.ProspectQuoteProductFeature.Commands.DeactivateProspectQuoteProductCommand
{
    public class DeactivateProspectQuoteProductCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }

    public class DeactivateProspectQuoteProductCommandHandler : IRequestHandler<DeactivateProspectQuoteProductCommand, Response<string>>
    {
        private readonly IRepositoryAsync<ProspectQuoteProduct> _repositoryAsync;
        private readonly IMapper _mapper;

        public DeactivateProspectQuoteProductCommandHandler(IRepositoryAsync<ProspectQuoteProduct> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<string>> Handle(DeactivateProspectQuoteProductCommand request, CancellationToken cancellationToken)
        {
            var checkQuoteProduct = await _repositoryAsync.GetByIdAsync(request.Id);
            if (checkQuoteProduct == null)
            {
                throw new KeyNotFoundException($"No se encontro la cotizacion con id {request.Id}");
            }
            if (checkQuoteProduct!.IsActive == false)
            {
                throw new ApiException("Esta cotizacion ya se encuentra inactiva.");
            }
            checkQuoteProduct!.IsActive = false;
            await _repositoryAsync.UpdateAsync(checkQuoteProduct);
            await _repositoryAsync.SaveChangesAsync();
            return new Response<string>("Eliminado exitosamente");

        }
    }
}
