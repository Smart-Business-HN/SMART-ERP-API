using AutoMapper;
using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpinionSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Company;

namespace SMART.ERP.Application.Features.OpinionFeature.Commands.CreateOpinionCommand
{
    public class CreateOpinionCommand : IRequest<Response<OpinionDto>>
    {
        public string Title { get; set; } = null!;
        public string? Image { get; set; }
        public string Observation { get; set; } = null!;
        public string Customer { get; set; } = null!;
        public string Charge { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class CreateOpinionCommandHandler : IRequestHandler<CreateOpinionCommand, Response<OpinionDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Opinion> _repositoryAsync;
        private readonly IRepositoryAsync<Company> _companyRepositoryAsync;

        public CreateOpinionCommandHandler(IMapper mapper, IRepositoryAsync<Opinion> repositoryAsync,
            IRepositoryAsync<Company> companyRepositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _companyRepositoryAsync = companyRepositoryAsync;
        }

        public async Task<Response<OpinionDto>> Handle(CreateOpinionCommand request, CancellationToken cancellationToken)
        {
            var checkIfExist = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterOpinionSpecification(request.Customer, null));
            if (checkIfExist != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Customer}");
            }
            var newRecord = _mapper.Map<Opinion>(request);
            var company = await _companyRepositoryAsync.ListAsync();
            if (company.Count() == 0)
                throw new ApiException("Debe registrar los datos de la empresa");
            newRecord.CompanyId = company[0].Id;
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<OpinionDto>(data);

            return new Response<OpinionDto>(dto, message: $"{request.Customer} creado exitosamente");
        }
    }
}
