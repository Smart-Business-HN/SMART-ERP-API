using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.TaxSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;


namespace SMART.ERP.Application.Features.TaxFeature.Commands.UpdateTaxCommand
{
    public class UpdateTaxCommand : IRequest<Response<TaxDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Rate { get; set; }
        public string TextForDocuments { get; set; } = null!;
    }
    public class UpdateTaxCommandHandler : IRequestHandler<UpdateTaxCommand, Response<TaxDto>>
    {
        private readonly IRepositoryAsync<Tax> _repositoryAsync;
        private readonly IMapper _mapper;
        public UpdateTaxCommandHandler(IRepositoryAsync<Tax> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<TaxDto>> Handle(UpdateTaxCommand request, CancellationToken cancellationToken)
        {
            var tax = await _repositoryAsync.GetByIdAsync(request.Id);
            if (tax == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var filterByName = await _repositoryAsync.FirstOrDefaultAsync(new FilterTaxByNameSpecification(request.Name, request.Id));
            if (filterByName != null)
            {
                throw new ApiException($"Ya existe una impuesto con el nombre {request.Name}");
            }
            tax.Name = request.Name;
            tax.Rate = request.Rate;
            tax.TextForDocuments = request.TextForDocuments;
            await _repositoryAsync.UpdateAsync(tax);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<TaxDto>(tax);
            return new Response<TaxDto>(dto, message: $"{tax.Name} actualizado correctamente");
        }
    }
}
