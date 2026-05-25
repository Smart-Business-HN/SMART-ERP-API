using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.LedgerAccount;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Helpers;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.LedgerAccountSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.LedgerAccountFeature.Commands.CreateLedgerAccountCommand
{
    public class CreateLedgerAccountCommand : IRequest<Response<LedgerAccountDto>>
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        /// <summary>Solo se usa para cuentas raíz (Tipo). En cuentas hijas se hereda del padre.</summary>
        public AccountType AccountType { get; set; }
        public int? ParentId { get; set; }
        public bool IsPostable { get; set; }
        public string? Description { get; set; }
        public int? ExpenseAccountId { get; set; }
        public int? IncomeAccountId { get; set; }

        public class CreateLedgerAccountCommandHandler : IRequestHandler<CreateLedgerAccountCommand, Response<LedgerAccountDto>>
        {
            private readonly IMapper _mapper;
            private readonly IJwtService _jwtService;
            private readonly IRepositoryAsync<LedgerAccount> _repositoryAsync;
            private readonly IOutputCacheStore _outputCacheStored;

            public CreateLedgerAccountCommandHandler(IMapper mapper, IJwtService jwtService,
                IRepositoryAsync<LedgerAccount> repositoryAsync, IOutputCacheStore outputCacheStored)
            {
                _mapper = mapper;
                _jwtService = jwtService;
                _repositoryAsync = repositoryAsync;
                _outputCacheStored = outputCacheStored;
            }

            public async Task<Response<LedgerAccountDto>> Handle(CreateLedgerAccountCommand request, CancellationToken cancellationToken)
            {
                var code = request.Code.Trim();
                var level = AccountCodeHelper.ResolveLevel(code);

                var duplicated = await _repositoryAsync.FirstOrDefaultAsync(new FilterLedgerAccountByCodeSpecification(code), cancellationToken);
                if (duplicated != null)
                    throw new ApiException($"Ya existe una cuenta con el código {code}.");

                AccountType accountType = request.AccountType;
                LedgerAccount? parent = null;

                if (request.ParentId.HasValue)
                {
                    parent = await _repositoryAsync.GetByIdAsync(request.ParentId.Value, cancellationToken)
                        ?? throw new ApiException($"No existe una cuenta padre con el Id {request.ParentId}.");
                    if (!code.StartsWith(parent.Code))
                        throw new ApiException($"El código {code} debe iniciar con el código del padre {parent.Code}.");
                    accountType = parent.AccountType;

                    // Una cuenta que pasa a tener subcuentas deja de ser imputable (partida doble:
                    // solo las hojas reciben movimientos). Se degrada automáticamente.
                    if (parent.IsPostable)
                    {
                        parent.IsPostable = false;
                        parent.ModificationDate = DateTime.Now;
                        parent.ModifiedBy = _jwtService.GetSubjectToken();
                        await _repositoryAsync.UpdateAsync(parent, cancellationToken);
                    }
                }
                else
                {
                    if (level != AccountLevel.Tipo)
                        throw new ApiException("Una cuenta sin padre debe ser de nivel Tipo (código de 1 dígito).");
                }

                var entity = new LedgerAccount
                {
                    Code = code,
                    Name = request.Name.Trim(),
                    AccountType = accountType,
                    Level = level,
                    NormalBalanceSide = AccountCodeHelper.ResolveNormalBalanceSide(accountType),
                    ParentId = request.ParentId,
                    IsPostable = request.IsPostable,
                    IsActive = true,
                    IsSystem = false,
                    Description = request.Description,
                    ExpenseAccountId = request.ExpenseAccountId,
                    IncomeAccountId = request.IncomeAccountId,
                    CreationDate = DateTime.Now,
                    CreatedBy = _jwtService.GetSubjectToken()
                };

                var data = await _repositoryAsync.AddAsync(entity, cancellationToken);
                await _outputCacheStored.EvictByTagAsync("cache_ledger_accounts", cancellationToken);

                var dto = _mapper.Map<LedgerAccountDto>(data);
                return new Response<LedgerAccountDto>(dto, $"Cuenta {data.Code} - {data.Name} creada exitosamente.");
            }
        }
    }
}
