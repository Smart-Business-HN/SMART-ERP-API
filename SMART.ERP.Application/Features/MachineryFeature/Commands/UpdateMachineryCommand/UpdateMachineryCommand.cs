using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Rootcloud;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.MachinerySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.MachineryFeature.Commands.UpdateMachineryCommand
{
    public class UpdateMachineryCommand : IRequest<Response<MachineryNoListObjectsDto>>
    {
        public int Id { get; set; }
        public string DeviceName { get; set; } = null!;
        public string SerialNum { get; set; } = null!;
        public string MachineTypeName { get; set; } = null!;
        public decimal Interval { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }
        public string Customer { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string? Status { get; set; }
        public string Province { get; set; } = null!;
        public decimal Milenage { get; set; }
        public bool InternalMaintenance { get; set; }
        public bool IsRootcloudActive { get; set; }
        public int RootcloudHistoricalId { get; set; }
        public int SubcategoryId { get; set; }
        public int BrandId { get; set; }
    }

    public class UpdateMachineryCommandHandler : IRequestHandler<UpdateMachineryCommand, Response<MachineryNoListObjectsDto>>
    {
        private readonly IRepositoryAsync<Machinery> _machineryRepositoryAsync;
        private readonly IRepositoryAsync<MachineryRootcloudHistorical> _historicalRepositoryAsync;
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Subcategory> _categoryRepositoryAsync;
        private readonly IRepositoryAsync<Brand> _brandRepositoryAsync;

        public UpdateMachineryCommandHandler(IRepositoryAsync<Machinery> machineryRepositoryAsync,
            IRepositoryAsync<MachineryRootcloudHistorical> historicalRepositoryAsync,
            IMapper mapper, IRepositoryAsync<Subcategory> categoryRepositoryAsync,
            IRepositoryAsync<Brand> brandRepositoryAsync)
        {
            _machineryRepositoryAsync = machineryRepositoryAsync;
            _historicalRepositoryAsync = historicalRepositoryAsync;
            _mapper = mapper;
            _categoryRepositoryAsync = categoryRepositoryAsync;
            _brandRepositoryAsync = brandRepositoryAsync;
        }
        public async Task<Response<MachineryNoListObjectsDto>> Handle(UpdateMachineryCommand request, CancellationToken cancellationToken)
        {
            var checkIfExistSerialNum = await _machineryRepositoryAsync.FirstOrDefaultAsync(new FilterBySerialNumMachinerySpecification(request.SerialNum, request.Id));
            if (checkIfExistSerialNum != null)
                throw new ApiException($"Ya existe una maquina con el numero de serie: {request.SerialNum}");

            var checkIfExistName = await _machineryRepositoryAsync.FirstOrDefaultAsync(new FilterByNameMachinerySpecification(request.DeviceName, request.Id));
            if (checkIfExistName != null)
                throw new ApiException($"Ya existe una maquina con el nombre: {request.SerialNum}");

            var machinery = await _machineryRepositoryAsync.GetByIdAsync(request.Id);
            if (machinery == null)
                throw new ApiException($"No se encontro ninguna maquina con el id: {request.Id}");

            var getCategory = await _categoryRepositoryAsync.GetByIdAsync(request.SubcategoryId);
            if (getCategory == null)
                throw new ApiException($"No se encontro ninguna categoria con el id: {request.SubcategoryId}");

            var getBrand = await _brandRepositoryAsync.GetByIdAsync(request.BrandId);
            if (getBrand == null)
                throw new ApiException($"No se encontro ninguna marca con el id: {request.BrandId}");

            machinery.Interval = request.Interval;
            machinery.Customer = request.Customer;
            machinery.DeviceName = request.DeviceName;
            machinery.SerialNum = request.SerialNum;
            machinery.Country = request.Country;
            machinery.MachineTypeName = request.MachineTypeName;
            machinery.Province = request.Province;
            machinery.IsRootcloudActive = request.IsRootcloudActive;
            machinery.BrandId = request.BrandId;
            machinery.ActiveMaintenance = request.InternalMaintenance;
            machinery.SubcategoryId = request.SubcategoryId;
            machinery.Status = request.Status == MachineryStatus.Activo.ToString()
                || request.Status == MachineryStatus.Inactivo.ToString()
                || request.Status == MachineryStatus.Retirado.ToString()
                ? request.Status : "N/A";

            await _machineryRepositoryAsync.UpdateAsync(machinery);
            await _machineryRepositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<MachineryResumeDto>(machinery);

            var getHistorical = await _historicalRepositoryAsync.GetByIdAsync(request.RootcloudHistoricalId);
            if (getHistorical == null)
                throw new ApiException($"No se enonctro ningun registro de horas con el id: {request.RootcloudHistoricalId}");

            if (request.Lat != 0 && request.Lat != 0)
            {
                if (getHistorical.Lat != request.Lat)
                {
                    getHistorical.Lat = request.Lat;
                }
                if (getHistorical.Lng != request.Lng)
                {
                    getHistorical.Lng = request.Lng;
                }
            }
            if (request.Milenage > 0)
                getHistorical.Milenage = request.Milenage;

            await _historicalRepositoryAsync.UpdateAsync(getHistorical);
            await _historicalRepositoryAsync.SaveChangesAsync();

            var newObject = _mapper.Map<MachineryNoListObjectsDto>(dto);
            newObject.MachineryFailureReport = dto.MachineryFailureReports?.FirstOrDefault();
            newObject.MachineryMaintenance = dto.MachineryMaintenances?.FirstOrDefault();
            newObject.MachineyRootcloudHistorical = dto.MachineyRootcloudHistoricals?.FirstOrDefault();
            return new Response<MachineryNoListObjectsDto>(newObject, "Se actualizo correctamente");
        }
    }
}
