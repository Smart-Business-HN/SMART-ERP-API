using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.DataSheetSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DataSheetFeature.Commands.UpdateDataSheetCommand
{
    public class UpdateDataSheetCommand : IRequest<Response<DataSheetDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public bool? IsOutstanding { get; set; }
    }

    public class UpdateDataSheetCommandHandler : IRequestHandler<UpdateDataSheetCommand, Response<DataSheetDto>>
    {
        private readonly IRepositoryAsync<DataSheet> _repositoryAsync;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public UpdateDataSheetCommandHandler(IMapper mapper, IRepositoryAsync<DataSheet> repositoryAsync,
            IJwtService jwtService)
        {
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
            _mapper = mapper;
        }
        public async Task<Response<DataSheetDto>> Handle(UpdateDataSheetCommand request, CancellationToken cancellationToken)
        {
            var dataSheet = await _repositoryAsync.GetByIdAsync(request.Id);
            if (dataSheet == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var filterByName = await _repositoryAsync.FirstOrDefaultAsync(
                    new FilterDataSheetSpecification(request.Name, request.Id));
            if (filterByName != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            else
            {
                dataSheet.Name = request.Name;
                dataSheet.IsActive = request.IsActive;
                dataSheet.ModificationDate = DateTime.Now;
                dataSheet.ModificatedBy = _jwtService.GetSubjectToken();
                await _repositoryAsync.UpdateAsync(dataSheet);
                await _repositoryAsync.SaveChangesAsync();
                var dto = _mapper.Map<DataSheetDto>(dataSheet);
                return new Response<DataSheetDto>(dto, message: $"{dataSheet.Name} actualizado correctamente");
            }
        }
    }
}
