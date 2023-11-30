using AutoMapper;
using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.BranchOfficeSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Company;
using Microsoft.AspNetCore.OutputCaching;

namespace SMART.ERP.Application.Features.BranchOfficeFeature.Commands.UpdateBranchOfficeCommand
{
    public class UpdateBranchOfficeCommand : IRequest<Response<BranchOfficeDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public float Lat { get; set; }
        public float Lng { get; set; }
        public bool IsActive { get; set; }
        public bool IsMainBranchOffice { get; set; }
    }

    public class UpdateBranchOfficeCommandHandler : IRequestHandler<UpdateBranchOfficeCommand, Response<BranchOfficeDto>>
    {
        private readonly IRepositoryAsync<BranchOffices> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IOutputCacheStore _outputCacheStored;

        public UpdateBranchOfficeCommandHandler(IRepositoryAsync<BranchOffices> repositoryAsync, IMapper mapper,
            IJwtService jwtService, IOutputCacheStore outputCacheStored)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _jwtService = jwtService;
            _outputCacheStored = outputCacheStored;
        }
        public async Task<Response<BranchOfficeDto>> Handle(UpdateBranchOfficeCommand request, CancellationToken cancellationToken)
        {
            var branchOffice = await _repositoryAsync.GetByIdAsync(request.Id);
            if (branchOffice == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }

            var filterByName = await _repositoryAsync.FirstOrDefaultAsync(
                    new FilterBranchOfficeSpecification(request.Name, request.Id, false));
            if (filterByName != null)
            {
                throw new ApiException($"Ya existe una sucursal con el nombre {request.Name}");
            }

            var checkIfExistMain = await _repositoryAsync.FirstOrDefaultAsync(
                    new FilterBranchOfficeSpecification(request.Name, request.Id, true));
            if (checkIfExistMain != null)
            {
                throw new ApiException($"Ya existe una sucursal principal");
            }

            branchOffice.Name = request.Name;
            branchOffice.Address = request.Address;
            branchOffice.Lat = request.Lat;
            branchOffice.Lng = request.Lng;
            branchOffice.PhoneNumber = request.PhoneNumber;
            branchOffice.Email = request.Email;
            branchOffice.IsActive = request.IsActive;
            branchOffice.ModificatedBy = _jwtService.GetSubjectToken();
            branchOffice.ModificationDate = DateTime.Now;
            branchOffice.IsMainBranchOffice = request.IsMainBranchOffice;
            await _repositoryAsync.UpdateAsync(branchOffice);
            await _repositoryAsync.SaveChangesAsync();
            await _outputCacheStored.EvictByTagAsync("cache_branchOffices", cancellationToken);
            var dto = _mapper.Map<BranchOfficeDto>(branchOffice);
            return new Response<BranchOfficeDto>(dto, message: $"{branchOffice.Name} actualizado correctamente");
        }
    }
}
