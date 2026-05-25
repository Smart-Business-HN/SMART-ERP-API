using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.JournalEntrySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.FiscalPeriodFeature.Commands.SetFiscalPeriodStatusCommand
{
    /// <summary>Abre o cierra un período fiscal. Al cerrar se bloquea la contabilización en esas fechas.</summary>
    public class SetFiscalPeriodStatusCommand : IRequest<Response<int>>
    {
        public int FiscalPeriodId { get; set; }
        public bool Close { get; set; }

        public class SetFiscalPeriodStatusCommandHandler : IRequestHandler<SetFiscalPeriodStatusCommand, Response<int>>
        {
            private readonly IJwtService _jwtService;
            private readonly IRepositoryAsync<FiscalPeriod> _repositoryAsync;
            private readonly IReadRepositoryAsync<JournalEntry> _journalRepository;
            private readonly IOutputCacheStore _outputCacheStored;

            public SetFiscalPeriodStatusCommandHandler(IJwtService jwtService, IRepositoryAsync<FiscalPeriod> repositoryAsync,
                IReadRepositoryAsync<JournalEntry> journalRepository, IOutputCacheStore outputCacheStored)
            {
                _jwtService = jwtService;
                _repositoryAsync = repositoryAsync;
                _journalRepository = journalRepository;
                _outputCacheStored = outputCacheStored;
            }

            public async Task<Response<int>> Handle(SetFiscalPeriodStatusCommand request, CancellationToken cancellationToken)
            {
                var period = await _repositoryAsync.GetByIdAsync(request.FiscalPeriodId, cancellationToken)
                    ?? throw new ApiException($"No existe un período fiscal con el Id {request.FiscalPeriodId}.");

                if (request.Close)
                {
                    var drafts = await _journalRepository.CountAsync(new FilterDraftJournalEntriesByPeriodSpecification(period.Id), cancellationToken);
                    if (drafts > 0)
                        throw new ApiException("No se puede cerrar el período: existen asientos en borrador. Contabilícelos o elimínelos primero.");

                    period.Status = FiscalPeriodStatus.Closed;
                    period.ClosedDate = DateTime.Now;
                    period.ClosedBy = _jwtService.GetSubjectToken();
                }
                else
                {
                    period.Status = FiscalPeriodStatus.Open;
                    period.ClosedDate = null;
                    period.ClosedBy = null;
                }
                period.ModificationDate = DateTime.Now;
                period.ModifiedBy = _jwtService.GetSubjectToken();

                await _repositoryAsync.UpdateAsync(period, cancellationToken);
                await _outputCacheStored.EvictByTagAsync("cache_fiscal_periods", cancellationToken);

                return new Response<int>(period.Id, request.Close ? $"Período {period.Name} cerrado." : $"Período {period.Name} reabierto.");
            }
        }
    }
}
