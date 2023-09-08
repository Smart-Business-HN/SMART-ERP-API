using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.TaxSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.TaxFeature.Commands.CreateTaxCommand
{
    public class CreateTaxCommand : IRequest<Response<TaxDto>>
    {
        public string Name { get; set; } = null!;
        public int Rate { get; set; }
        public string TextForDocuments { get; set; } = null!;
    }
    public class CreateTaxCommandHandler : IRequestHandler<CreateTaxCommand, Response<TaxDto>>
    {
        private readonly IRepositoryAsync<Tax> _repositoryAsync;
        private readonly IMapper _mapper;
        public CreateTaxCommandHandler(IRepositoryAsync<Tax> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }
        public async Task<Response<TaxDto>> Handle(CreateTaxCommand request, CancellationToken cancellationToken)
        {
            var checkTax = await _repositoryAsync.FirstOrDefaultAsync(new FilterTaxByNameSpecification(request.Name,null));
            if (checkTax != null)
            {
                throw new ApiException($"Ya existe una region con el nombre {request.Name}");
            }
            var newRecord = _mapper.Map<Tax>(request);
            var response = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<TaxDto>(response);
            return new Response<TaxDto>(dto, $"Impuesto {request.Name} creado exitosamente.");
        }
    }
}
