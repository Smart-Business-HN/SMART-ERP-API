using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.MonthlySaleDeclaration;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.MonthlySaleDeclarationSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.MonthlySaleDeclarationFeature.Commands.DeleteDeclaredSaleInvoiceCommand
{
    public class DeleteDeclaredSaleInvoiceCommand : IRequest<Response<MonthlySaleDeclarationDto>>
    {
        public int Id { get; set; }
    }

    public class DeleteDeclaredSaleInvoiceCommandHandler : IRequestHandler<DeleteDeclaredSaleInvoiceCommand, Response<MonthlySaleDeclarationDto>>
    {
        private readonly IRepositoryAsync<DeclaredSaleInvoice> _declaredSaleInvoiceRepositoryAsync;
        private readonly IRepositoryAsync<MonthlySaleDeclaration> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IOutputCacheStore _outputCacheStored;

        public DeleteDeclaredSaleInvoiceCommandHandler(
            IRepositoryAsync<DeclaredSaleInvoice> declaredSaleInvoiceRepositoryAsync,
            IRepositoryAsync<MonthlySaleDeclaration> repositoryAsync,
            IMapper mapper,
            IOutputCacheStore outputCacheStored)
        {
            _declaredSaleInvoiceRepositoryAsync = declaredSaleInvoiceRepositoryAsync;
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _outputCacheStored = outputCacheStored;
        }

        public async Task<Response<MonthlySaleDeclarationDto>> Handle(DeleteDeclaredSaleInvoiceCommand request, CancellationToken cancellationToken)
        {
            var declaredInvoice = await _declaredSaleInvoiceRepositoryAsync.GetByIdAsync(request.Id);
            if (declaredInvoice == null)
            {
                throw new KeyNotFoundException($"No se encontro la factura declarada con el id {request.Id}");
            }

            var declaration = await _repositoryAsync.FirstOrDefaultAsync(new GetMonthlySaleDeclarationByIdSpecification(declaredInvoice.MonthlySaleDeclarationId));
            if (declaration == null)
            {
                throw new KeyNotFoundException($"No se encontro la declaracion mensual.");
            }

            // Recalcular totales restando los valores de la factura eliminada
            declaration.TotalInvoices--;
            declaration.Exempt -= declaredInvoice.Exempt;
            declaration.Exonerated -= declaredInvoice.Exonerated;
            declaration.TaxedAt15Percent -= declaredInvoice.TaxedAt15Percent;
            declaration.TaxedAt18Percent -= declaredInvoice.TaxedAt18Percent;
            declaration.Taxes15Percent -= declaredInvoice.Taxes15Percent;
            declaration.Taxes18Percent -= declaredInvoice.Taxes18Percent;
            declaration.TotalTaxes = declaration.Taxes15Percent + declaration.Taxes18Percent;
            declaration.Total = declaration.Exempt + declaration.Exonerated + declaration.TaxedAt15Percent + declaration.TaxedAt18Percent + declaration.Taxes15Percent + declaration.Taxes18Percent;
            declaration.DeclaredSaleInvoices = null;
            // DeleteAsync internamente llama SaveChangesAsync, lo cual persiste
            // tanto la eliminacion como las modificaciones a declaration (tracked entity)
            await _declaredSaleInvoiceRepositoryAsync.DeleteAsync(declaredInvoice);
            await _declaredSaleInvoiceRepositoryAsync.SaveChangesAsync();
            await _repositoryAsync.UpdateAsync(declaration);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_monthlySaleDeclaration", cancellationToken);

            // Recargar la declaracion con los datos actualizados
            var updatedDeclaration = await _repositoryAsync.FirstOrDefaultAsync(new GetMonthlySaleDeclarationByIdSpecification(declaration.Id));
            var dto = _mapper.Map<MonthlySaleDeclarationDto>(updatedDeclaration);
            return new Response<MonthlySaleDeclarationDto>(dto);
        }
    }
}
