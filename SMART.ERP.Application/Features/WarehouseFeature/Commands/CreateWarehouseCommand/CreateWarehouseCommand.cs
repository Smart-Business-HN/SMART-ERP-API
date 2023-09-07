using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Address;
using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.DTOs.User;
using SMART.ERP.Application.DTOs.Warehouse;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.WarehouseFeature.Commands.CreateWarehouseCommand
{
    public class CreateWarehouseCommand : IRequest<Response<WarehouseDto>>
    {
        public string Name { get; set; } 
        public string? Address { get; set; }
        public Guid? UserId { get; set; }
        public int BranchOfficeId { get; set; }
        public bool? IsGeneralWarehouse { get; set; }
        public int? CityId { get; set; }
    }
    public class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, Response<WarehouseDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Warehouse> _repositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<BranchOffices> _branchOfficesRepositoryAsync;
        private readonly IRepositoryAsync<City> _cityRepositoryAsync; 
        private readonly IJwtService _jwtService;
        public CreateWarehouseCommandHandler(IMapper mapper, IRepositoryAsync<Warehouse> repositoryAsync, IRepositoryAsync<User> user, IRepositoryAsync<BranchOffices> branchOffice, IRepositoryAsync<City> city, IJwtService jwtService)
        {
            _mapper = mapper;
            _branchOfficesRepositoryAsync = branchOffice;
            _cityRepositoryAsync = city;
            _jwtService = jwtService;
            _repositoryAsync = repositoryAsync;
            _userRepositoryAsync = user;
        }
        public async Task<Response<WarehouseDto>> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
        {
            var isBranchOfficeExist = await _branchOfficesRepositoryAsync.GetByIdAsync(request.BranchOfficeId);
            if (isBranchOfficeExist == null)
            {
                throw new ApiException($"No existe una Sucursal con el Id {request.BranchOfficeId}");
            }
            if(request.CityId.HasValue)
            {
                var isCityExist = await _cityRepositoryAsync.GetByIdAsync(request.CityId.Value);
                if (isCityExist == null)
                {
                    throw new ApiException($"No existe una ciudad con el Id {request.CityId}");
                }
            }
            if (request.UserId.HasValue)
            {
                var isUserExist = await _userRepositoryAsync.GetByIdAsync(request.UserId.Value, cancellationToken);
                if (isUserExist == null)
                {
                    throw new ApiException($"No existe un usuario con el Id {request.UserId}");
                }
            }
            var newRecord = _mapper.Map<Warehouse>(request);
            newRecord.CreationDate = DateTime.Now;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<WarehouseDto>(data);
            return new Response<WarehouseDto>(dto, message: $"{request.Name} creado exitosamente");

        }
    }
}
