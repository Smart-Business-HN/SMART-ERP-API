using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InterestLevelSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InterestLevelFeature.Commands.CreateInterestLevelCommand
{
    public class CreateInterestLevelCommand : IRequest<Response<InterestLevelDto>>
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class CreateInteresLevelCommandHandler : IRequestHandler<CreateInterestLevelCommand, Response<InterestLevelDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<InterestLevel> _repositoryAsync;

        public CreateInteresLevelCommandHandler(IMapper mapper, IRepositoryAsync<InterestLevel> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<InterestLevelDto>> Handle(CreateInterestLevelCommand request, CancellationToken cancellationToken)
        {
            var checkIfExistByName = await _repositoryAsync.FirstOrDefaultAsync(
                new FilterInterestLevelSpecification(request.Name, null));
            if (checkIfExistByName != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }

            var newRecord = _mapper.Map<InterestLevel>(request);
            newRecord.CreationDate = DateTime.Now;
            var data = await _repositoryAsync.AddAsync(newRecord);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<InterestLevelDto>(data);

            return new Response<InterestLevelDto>(dto, message: $"{request.Name} creado exitosamente");
        }
    }
}
