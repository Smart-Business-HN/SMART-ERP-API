using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Warehouse;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.WarehouseFeature.Commands.UpdateWarehouseCommand
{
    public class UpdateWarehouseCommand : IRequest<Response<WarehouseDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public Guid? UserId { get; set; }
        public int BranchOfficeId { get; set; }
        public bool? IsGeneralWarehouse { get; set; }
        public int? CityId { get; set; }
    }
    public class UpdateWarehouseCommandHandler : IRequestHandler<UpdateWarehouseCommand, Response<WarehouseDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Warehouse> _repositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<BranchOffices> _branchOfficesRepositoryAsync;
        private readonly IRepositoryAsync<City> _cityRepositoryAsync;
        private readonly IJwtService _jwtService;
        public UpdateWarehouseCommandHandler(IMapper mapper, IRepositoryAsync<Warehouse> repositoryAsync, IRepositoryAsync<User> userRepositoryAsync, IRepositoryAsync<BranchOffices> branchOfficesRepositoryAsync, IRepositoryAsync<City> cityRepositoryAsync, IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _branchOfficesRepositoryAsync = branchOfficesRepositoryAsync;
            _cityRepositoryAsync = cityRepositoryAsync;
            _jwtService = jwtService;
        }
        public async Task<Response<WarehouseDto>> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
        {
            var warehouse = await _repositoryAsync.GetByIdAsync(request.Id);
            if (warehouse == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var existBranchOffice = await _branchOfficesRepositoryAsync.GetByIdAsync(request.BranchOfficeId);
            if (existBranchOffice == null)
            {
                throw new KeyNotFoundException($"No se encontro ninguna ciudad con el id {request.CityId}");
            }
            if(request.CityId.HasValue)
            {
                var existCity = await _cityRepositoryAsync.GetByIdAsync(request.CityId.Value);
                if (existCity == null)
                {
                    throw new KeyNotFoundException($"No se encontro ninguna ciudad con el id {request.CityId}");
                }
                warehouse.CityId = request.CityId;
            }
            if(request.UserId.HasValue)
            {
                var existUser = await _userRepositoryAsync.GetByIdAsync(request.UserId.Value);
                if(existUser == null)
                {
                    throw new KeyNotFoundException($"No se encontro ninguna ciudad con el id {request.CityId}");
                }
                warehouse.UserId = request.UserId;
            }
            if(request.Name != warehouse.Name)
            {
                warehouse.Name = request.Name;
            }
            warehouse.BranchOfficeId = request.BranchOfficeId;
            warehouse.ModificatedBy = _jwtService.GetSubjectToken();
            warehouse.ModificationDate = DateTime.Now;
            await _repositoryAsync.UpdateAsync(warehouse);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<WarehouseDto>(warehouse);
            return new Response<WarehouseDto> (dto, $"{request.Name} actualizado exitosamente");
        }
    }
}
