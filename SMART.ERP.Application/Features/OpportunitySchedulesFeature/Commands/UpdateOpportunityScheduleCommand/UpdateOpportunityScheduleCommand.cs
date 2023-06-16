using MediatR;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunityScheduleSpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunitySchedulesFeature.Commands.UpdateOpportunityScheduleCommand
{
    public class UpdateOpportunityScheduleCommand : IRequest<Response<string>>
    {
        public Guid UserId { get; set; }
        public int Option { get; set; }
    }

    public class UpdateOpportunityScheduleCommandHandler : IRequestHandler<UpdateOpportunityScheduleCommand, Response<string>>
    {
        private readonly IRepositoryAsync<OpportunitySchedules> _repositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;

        public UpdateOpportunityScheduleCommandHandler(IRepositoryAsync<OpportunitySchedules> repositoryAsync, IRepositoryAsync<User> userRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
        }

        public async Task<Response<string>> Handle(UpdateOpportunityScheduleCommand request, CancellationToken cancellationToken)
        {
            if (request.Option < 0 && request.Option > 6)
            {
                throw new ApiException("Opción invalida");
            }
            var checkUser = await _userRepositoryAsync.FirstOrDefaultAsync(new UserIncludesSpecification(request.UserId, null));
            if (checkUser == null)
            {
                throw new KeyNotFoundException("Usuario invalido");
            }
            if (checkUser.Role!.Name != "Manager")
            {
                throw new ApiException("Caracteristica exclusiva solo para Managers.");
            }
            var checkSchedule = await _repositoryAsync.FirstOrDefaultAsync(new FilterOpportunityScheduleByUserSpecification(request.UserId));
            if (checkSchedule == null)
            {
                var newRecord = new OpportunitySchedules();
                newRecord.UserId = request.UserId;
                newRecord.OpportunityAge = request.Option;
                await _repositoryAsync.AddAsync(newRecord);
                await _repositoryAsync.SaveChangesAsync();

                return new Response<string>("Actualizado exitosamente");
            }
            else
            {
                checkSchedule.OpportunityAge = request.Option;
                await _repositoryAsync.UpdateAsync(checkSchedule);
                await _repositoryAsync.SaveChangesAsync();
                return new Response<string>("Actualizado exitosamente");
            }
        }
    }
}
