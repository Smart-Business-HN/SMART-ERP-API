using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.InterestLevelSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.InterestLevelFeature.Commands.UpdateInterestLevelCommand
{
    public class UpdateInterestLevelCommand : IRequest<Response<InterestLevelDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class UpdateInterestLevelCommandHandle : IRequestHandler<UpdateInterestLevelCommand, Response<InterestLevelDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<InterestLevel> _repositoryAsync;

        public UpdateInterestLevelCommandHandle(IMapper mapper, IRepositoryAsync<InterestLevel> repositoryAsync)
        {
            _mapper = mapper;
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<InterestLevelDto>> Handle(UpdateInterestLevelCommand request, CancellationToken cancellationToken)
        {
            var interestLevel = await _repositoryAsync.GetByIdAsync(request.Id);
            if (interestLevel == null)
            {
                throw new KeyNotFoundException($"No se encontro ningun registro con el id {request.Id}");
            }
            var checkIfExistByName = await _repositoryAsync.FirstOrDefaultAsync(new FilterInterestLevelSpecification(request.Name, request.Id));
            if (checkIfExistByName != null)
            {
                throw new ApiException($"Ya existe un registro con el nombre {request.Name}");
            }

            interestLevel.Name = request.Name;
            interestLevel.IsActive = request.IsActive;
            await _repositoryAsync.UpdateAsync(interestLevel);
            await _repositoryAsync.SaveChangesAsync();
            var dto = _mapper.Map<InterestLevelDto>(interestLevel);
            return new Response<InterestLevelDto>(dto, message: $"{interestLevel.Name} actualizado correctamente");
        }
    }
}
