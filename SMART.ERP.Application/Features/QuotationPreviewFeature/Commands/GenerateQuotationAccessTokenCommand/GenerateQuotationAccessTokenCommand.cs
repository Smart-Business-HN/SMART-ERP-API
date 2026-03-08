using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.QuotationPreviewFeature.Commands.GenerateQuotationAccessTokenCommand
{
    public class GenerateQuotationAccessTokenCommand : IRequest<Response<string>>
    {
        public int QuotationId { get; set; }
    }

    public class GenerateQuotationAccessTokenCommandHandler : IRequestHandler<GenerateQuotationAccessTokenCommand, Response<string>>
    {
        private readonly IRepositoryAsync<Quotation> _repositoryAsync;

        public GenerateQuotationAccessTokenCommandHandler(IRepositoryAsync<Quotation> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<string>> Handle(GenerateQuotationAccessTokenCommand request, CancellationToken cancellationToken)
        {
            var quotation = await _repositoryAsync.GetByIdAsync(request.QuotationId);
            if (quotation == null)
            {
                throw new ApiException($"No se encontró la cotización con el id {request.QuotationId}");
            }

            // If already has a token, return existing one
            if (quotation.AccessToken.HasValue)
            {
                return new Response<string>(quotation.AccessToken.Value.ToString(), "Esta cotización ya tiene un enlace de acceso.");
            }

            quotation.AccessToken = Guid.NewGuid();
            quotation.AccessTokenGeneratedDate = DateTime.Now;

            await _repositoryAsync.UpdateAsync(quotation);
            await _repositoryAsync.SaveChangesAsync();

            return new Response<string>(quotation.AccessToken.Value.ToString(), "Enlace de acceso generado exitosamente.");
        }
    }
}
