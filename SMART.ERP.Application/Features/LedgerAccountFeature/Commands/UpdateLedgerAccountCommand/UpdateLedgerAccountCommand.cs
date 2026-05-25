using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.LedgerAccount;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Features.LedgerAccountFeature.Commands.UpdateLedgerAccountCommand
{
    public class UpdateLedgerAccountCommand : IRequest<Response<LedgerAccountDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsPostable { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
        public int? ExpenseAccountId { get; set; }
        public int? IncomeAccountId { get; set; }

        public class UpdateLedgerAccountCommandHandler : IRequestHandler<UpdateLedgerAccountCommand, Response<LedgerAccountDto>>
        {
            private readonly IMapper _mapper;
            private readonly IJwtService _jwtService;
            private readonly IRepositoryAsync<LedgerAccount> _repositoryAsync;
            private readonly IOutputCacheStore _outputCacheStored;

            public UpdateLedgerAccountCommandHandler(IMapper mapper, IJwtService jwtService,
                IRepositoryAsync<LedgerAccount> repositoryAsync, IOutputCacheStore outputCacheStored)
            {
                _mapper = mapper;
                _jwtService = jwtService;
                _repositoryAsync = repositoryAsync;
                _outputCacheStored = outputCacheStored;
            }

            public async Task<Response<LedgerAccountDto>> Handle(UpdateLedgerAccountCommand request, CancellationToken cancellationToken)
            {
                var entity = await _repositoryAsync.GetByIdAsync(request.Id, cancellationToken)
                    ?? throw new ApiException($"No existe una cuenta con el Id {request.Id}.");

                // Si la cuenta tiene hijos no puede volverse imputable.
                if (request.IsPostable)
                {
                    var children = await _repositoryAsync.CountAsync(
                        new Specifications.LedgerAccountSpecification.FilterLedgerAccountChildrenSpecification(entity.Id), cancellationToken);
                    if (children > 0)
                        throw new ApiException("Una cuenta con subcuentas no puede marcarse como imputable.");
                }

                entity.Name = request.Name.Trim();
                entity.IsPostable = request.IsPostable;
                entity.IsActive = request.IsActive;
                entity.Description = request.Description;
                entity.ExpenseAccountId = request.ExpenseAccountId;
                entity.IncomeAccountId = request.IncomeAccountId;
                entity.ModificationDate = DateTime.Now;
                entity.ModifiedBy = _jwtService.GetSubjectToken();

                await _repositoryAsync.UpdateAsync(entity, cancellationToken);
                await _outputCacheStored.EvictByTagAsync("cache_ledger_accounts", cancellationToken);

                var dto = _mapper.Map<LedgerAccountDto>(entity);
                return new Response<LedgerAccountDto>(dto, $"Cuenta {entity.Code} actualizada exitosamente.");
            }
        }
    }
}
