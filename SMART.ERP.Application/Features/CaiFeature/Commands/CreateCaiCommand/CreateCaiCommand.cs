using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.Cai;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CaiSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;


namespace SMART.ERP.Application.Features.CaiFeature.Commands.CreateCaiCommand
{
    public class CreateCaiCommand : IRequest<Response<CaiDto>>
    {
        public string Name { get; set; } = null!;
        public int? BranchOfficeId { get; set; }
        public string Identificator { get; set; } = null!;
        public string Prefix { get; set; } = null!;
        public int StartCorrelative { get; set; }
        public int EndCorrelative { get; set; }
        public int? CurrentCorrelative { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool IsGeneralCai { get; set; }
        public bool IsActive { get; set; }
    }
    public class CreateCaiCommandHandler : IRequestHandler<CreateCaiCommand, Response<CaiDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Cai> _repositoryAsync;
        private readonly IRepositoryAsync<BranchOffices> _branchOfficeRepositoryAsync;
        private readonly IOutputCacheStore _outputCacheStored;

        public CreateCaiCommandHandler(IMapper mapper, IRepositoryAsync<Cai> repositoryAsync, IRepositoryAsync<BranchOffices> branchOfficeRepositoryAsync, IOutputCacheStore outputCacheStore)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _branchOfficeRepositoryAsync = branchOfficeRepositoryAsync;
            _outputCacheStored = outputCacheStore;
        }
        public async Task<Response<CaiDto>> Handle(CreateCaiCommand request, CancellationToken cancellationToken)
        {

            var filterByIdentificator = await _repositoryAsync.FirstOrDefaultAsync(new FilterCaiByIdentificatorSpecification(request.Identificator));
            if (filterByIdentificator != null)
            {
                throw new ApiException($"Ya existe un CAI con el identificador {request.Identificator}");
            }
            if (request.BranchOfficeId != null)
            {
                var branchOffice = await _branchOfficeRepositoryAsync.GetByIdAsync((int)request.BranchOfficeId);
                if (branchOffice == null)
                {
                    throw new ApiException($"No existe una sucursal con el id {request.BranchOfficeId}");
                }
            }
            if (request.CurrentCorrelative > request.EndCorrelative)
            {
                throw new Exception("El correlativo actual no puede ser mayor al correlativo final");
            }
            if (request.CurrentCorrelative < request.StartCorrelative)
            {
                throw new Exception("El correlativo actual no puede ser menor al correlativo inicial");
            }
            var newRecord = _mapper.Map<Cai>(request);
            newRecord.AvailableInvoices = request.EndCorrelative - request.StartCorrelative;
            newRecord.CurrentCorrelative = request.StartCorrelative;
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_cais", cancellationToken);
            var dto = _mapper.Map<CaiDto>(data);
            return new Response<CaiDto>(dto, message: $"{request.Name} creado exitosamente");

        }
    }
}
