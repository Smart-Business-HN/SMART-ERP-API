using AutoMapper;
using MediatR;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.CustomerSpecification;
using SMART.ERP.Application.Specifications.InterestLevelSpecification;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.OpportunityFeature.Commands.UpdateOpportunityCommand
{
    public class UpdateOpportunityCommand : IRequest<Response<OpportunityDto>>
    {
        public int Id { get; set; }
        public decimal Budget { get; set; }
        public DateTime? ExpectedClosingDate { get; set; }
        public DateTime? ClosingDate { get; set; }
        public int ProbabilityPercentage { get; set; }
        public string? Description { get; set; }
        public bool ApplyOnCredit { get; set; }
        public string? RecommendedBy { get; set; }
        public string? OpportunityType { get; set; }
        public int? TypeOriginId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsActive { get; set; }
        public int? LossReasonId { get; set; }
        public int? WinReasonId { get; set; }
        public Guid? UserId { get; set; }
        public int? OpportunityStepId { get; set; }
    }

    public class UpdateOpportunityCommandHandler : IRequestHandler<UpdateOpportunityCommand, Response<OpportunityDto>>
    {
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<InterestLevel> _interestRepositoryAsync;
        private readonly IRepositoryAsync<TypeOrigin> _originRepositoryAsync;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IRepositoryAsync<LossReason> _lossRepositoryAsync;
        private readonly IRepositoryAsync<WinReason> _winRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<OpportunityStep> _stepRepositoryAsync;

        public UpdateOpportunityCommandHandler(IRepositoryAsync<Opportunity> repositoryAsync, IMapper mapper, IRepositoryAsync<InterestLevel> interestRepositoryAsync,
            IRepositoryAsync<TypeOrigin> originRepositoryAsync, IRepositoryAsync<Customer> customerRepositoryAsync, IRepositoryAsync<LossReason> lossRepositoryAsync,
            IRepositoryAsync<WinReason> winRepositoryAsync, IRepositoryAsync<User> userRepositoryAsync, IRepositoryAsync<OpportunityStep> stepRepositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
            _interestRepositoryAsync = interestRepositoryAsync;
            _originRepositoryAsync = originRepositoryAsync;
            _customerRepositoryAsync = customerRepositoryAsync;
            _lossRepositoryAsync = lossRepositoryAsync;
            _winRepositoryAsync = winRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _stepRepositoryAsync = stepRepositoryAsync;
        }

        public async Task<Response<OpportunityDto>> Handle(UpdateOpportunityCommand request, CancellationToken cancellationToken)
        {
            var existOpportunity = await _repositoryAsync.GetByIdAsync(request.Id);
            if (existOpportunity == null)
            {
                throw new KeyNotFoundException($"No se encontro la oportunidad con id {request.Id}");
            }

            if (request.CreationDate.Date > DateTime.Now.Date)
            {
                throw new ApiException("La fecha de inicio no debe ser mayor a la fecha de hoy.");
            }

            var existCustomer = await _customerRepositoryAsync.FirstOrDefaultAsync(new FilterCustomerByMasterIdSpecification(request.CustomerId));
            if (existCustomer == null)
            {
                throw new KeyNotFoundException($"No se encontro el cliente con id {request.CustomerId}");
            }
            if (request.TypeOriginId != null && request.TypeOriginId != 0)
            {
                var existTypeOrigin = await _originRepositoryAsync.GetByIdAsync((int)request.TypeOriginId);
                if (existTypeOrigin == null)
                {
                    throw new KeyNotFoundException($"No se encontro el tipo de origen con id {request.TypeOriginId}");
                }
                existOpportunity.TypeOriginId = request.TypeOriginId;
            }
            else
            {
                existOpportunity.TypeOriginId = null;
            }
            if (request.LossReasonId != null && request.LossReasonId != 0)
            {
                var checkIfLossExist = await _lossRepositoryAsync.GetByIdAsync((int)request.LossReasonId);
                if (checkIfLossExist == null)
                {
                    throw new KeyNotFoundException($"No se encontro la razon de perdida con id {request.LossReasonId}");
                }
                existOpportunity.LossReasonId = request.LossReasonId;

            }
            else
            {
                existOpportunity.LossReasonId = null;
            }

            if (request.WinReasonId != null && request.WinReasonId != 0)
            {
                var checkIfWinExist = await _winRepositoryAsync.GetByIdAsync((int)request.WinReasonId);
                if (checkIfWinExist == null)
                {
                    throw new KeyNotFoundException($"No se encontro la razon de ganancia con id {request.LossReasonId}");
                }
                existOpportunity.WinReasonId = request.WinReasonId;
            }
            else
            {
                existOpportunity.WinReasonId = null;
            }

            if (request.ProbabilityPercentage >= 0 && request.ProbabilityPercentage <= 35)
            {
                var interestLevel = await _interestRepositoryAsync.FirstOrDefaultAsync(new FilterInterestLevelSpecification("Bajo", null));
                existOpportunity.InterestLevelId = interestLevel!.Id;
            }
            else if (request.ProbabilityPercentage >= 36 && request.ProbabilityPercentage <= 65)
            {
                var interestLevel = await _interestRepositoryAsync.FirstOrDefaultAsync(new FilterInterestLevelSpecification("Medio", null));
                existOpportunity.InterestLevelId = interestLevel!.Id;
            }
            else if (request.ProbabilityPercentage >= 66 && request.ProbabilityPercentage <= 100)
            {
                var interestLevel = await _interestRepositoryAsync.FirstOrDefaultAsync(new FilterInterestLevelSpecification("Alto", null));
                existOpportunity.InterestLevelId = interestLevel!.Id;
            }

            if (request.ClosingDate != null)
            {
                var steps = await _stepRepositoryAsync.ListAsync();
                var currentStep = steps.Find(x => x.Id == existOpportunity.OpportunityStepId)!;
                if (currentStep.Name != "Ganado" && currentStep.Name != "Perdido" && currentStep.Name != "Abandonado")
                {
                    throw new ApiException("No puedes establecer una fecha de cierre en una oportunidad activa.");
                }
                existOpportunity.ClosingDate = request.ClosingDate;
            }
            if (request.OpportunityStepId != null)
            {
                var checkStep = await _stepRepositoryAsync.GetByIdAsync((int)request.OpportunityStepId);
                if (checkStep == null)
                {
                    throw new ApiException($"No se encontro el paso con id {request.OpportunityStepId}");
                }
                if (checkStep.Name == "Ganado" || checkStep.Name == "Perdido" || checkStep.Name == "Abandonado")
                {
                    if (existOpportunity.QtyItems == 0)
                    {
                        throw new ApiException("No es posible mover una oportunidad sin productos a este paso.");
                    }
                    if (request.WinReasonId == null && request.LossReasonId == null)
                    {
                        throw new ApiException($"Debes proveer una razón para esta oportunidad.");
                    }
                    if (existOpportunity.OpportunityStepId != request.OpportunityStepId)
                    {
                        existOpportunity.ClosingDate = DateTime.Now;
                    }
                }
                else
                {
                    existOpportunity.ClosingDate = null;
                    existOpportunity.WinReasonId = null;
                    existOpportunity.LossReasonId = null;
                }
                if (request.OpportunityStepId != existOpportunity.OpportunityStepId)
                {
                    var previousContainer = await _repositoryAsync.ListAsync(new FilterOpportunityByStepSpecification(existOpportunity.OpportunityStepId, null, existOpportunity.Position, null, existOpportunity.UserId));
                    var currentContainer = await _repositoryAsync.ListAsync(new FilterOpportunityByStepSpecification((int)request.OpportunityStepId, null, null, 1, existOpportunity.UserId));
                    if (previousContainer.Count > 0)
                    {
                        foreach (var previousItem in previousContainer)
                        {
                            previousItem.Position -= 1;
                        }
                        await _repositoryAsync.UpdateRangeAsync(previousContainer);
                        await _repositoryAsync.SaveChangesAsync();
                    }
                    if (currentContainer.Count > 0)
                    {
                        foreach (var currentItem in currentContainer)
                        {
                            currentItem.Position += 1;
                        }
                        await _repositoryAsync.UpdateRangeAsync(currentContainer);
                        await _repositoryAsync.SaveChangesAsync();
                    }
                    existOpportunity.Position = 1;
                    existOpportunity.OpportunityStepId = (int)request.OpportunityStepId;
                }
            }

            existOpportunity.CustomerId = existCustomer!.Id;
            existOpportunity.TypeOriginId = request.TypeOriginId;
            existOpportunity.Budget = request.Budget;
            existOpportunity.ExpectedClosingDate = request.ExpectedClosingDate;
            existOpportunity.ProbabilityPercentage = request.ProbabilityPercentage;
            existOpportunity.Description = request.Description;
            existOpportunity.ApplyOnCredit = request.ApplyOnCredit;
            existOpportunity.RecommendedBy = request.RecommendedBy;
            existOpportunity.CreationDate = request.CreationDate;
            existOpportunity.IsActive = request.IsActive;
            existOpportunity.OpportunityType = request.OpportunityType;
            if (request.UserId != null)
            {
                var checkUser = await _userRepositoryAsync.GetByIdAsync((Guid)request.UserId);
                if (checkUser == null)
                {
                    throw new KeyNotFoundException($"No se encontro el usuario con id {request.UserId}");
                }
                existOpportunity.UserId = (Guid)request.UserId;
            }

            await _repositoryAsync.UpdateAsync(existOpportunity);
            await _repositoryAsync.SaveChangesAsync();
            var data = await _repositoryAsync.FirstOrDefaultAsync(new OpportunityIncludesSpecification(request.Id, null));
            var dto = _mapper.Map<OpportunityDto>(data);
            return new Response<OpportunityDto>(dto, $"{dto.Code} actualizado correctamente");

        }
    }
}
