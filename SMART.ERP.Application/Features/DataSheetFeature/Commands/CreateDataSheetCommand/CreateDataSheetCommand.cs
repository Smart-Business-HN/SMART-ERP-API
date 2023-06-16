using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.DataSheetSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.DataSheetFeature.Commands.CreateDataSheetCommand
{
    public class CreateDataSheetCommand : IRequest<Response<DataSheetDto>>
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public bool? IsOutstanding { get; set; }
    }

    public class CreateDataSheetCommandHandler : IRequestHandler<CreateDataSheetCommand, Response<DataSheetDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<DataSheet> _repositoryAsync;
        private readonly IJwtService _jwtService;

        public CreateDataSheetCommandHandler(IMapper mapper, IRepositoryAsync<DataSheet> repositoryAsync,
            IJwtService jwtService)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
            _jwtService = jwtService;
        }

        public async Task<Response<DataSheetDto>> Handle(CreateDataSheetCommand request, CancellationToken cancellationToken)
        {
            var checkIfExist = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterDataSheetSpecification(request.Name, null));
            if (checkIfExist != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }
            var newRecord = _mapper.Map<DataSheet>(request);
            newRecord.CreationDate = DateTime.Now;
            newRecord.CreatedBy = _jwtService.GetSubjectToken();
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<DataSheetDto>(data);

            return new Response<DataSheetDto>(dto, message: $"{request.Name} creado exitosamente");
        }
    }
}
