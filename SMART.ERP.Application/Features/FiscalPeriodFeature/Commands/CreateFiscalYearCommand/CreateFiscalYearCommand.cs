using System.Globalization;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.OutputCaching;
using SMART.ERP.Application.DTOs.FiscalPeriod;
using SMART.ERP.Application.Exceptions;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Specifications.FiscalPeriodSpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Features.FiscalPeriodFeature.Commands.CreateFiscalYearCommand
{
    /// <summary>Crea un ejercicio fiscal y genera automáticamente sus 12 períodos mensuales abiertos.</summary>
    public class CreateFiscalYearCommand : IRequest<Response<FiscalYearDto>>
    {
        public int Year { get; set; }

        public class CreateFiscalYearCommandHandler : IRequestHandler<CreateFiscalYearCommand, Response<FiscalYearDto>>
        {
            private readonly IMapper _mapper;
            private readonly IJwtService _jwtService;
            private readonly IRepositoryAsync<FiscalYear> _repositoryAsync;
            private readonly IOutputCacheStore _outputCacheStored;

            public CreateFiscalYearCommandHandler(IMapper mapper, IJwtService jwtService,
                IRepositoryAsync<FiscalYear> repositoryAsync, IOutputCacheStore outputCacheStored)
            {
                _mapper = mapper;
                _jwtService = jwtService;
                _repositoryAsync = repositoryAsync;
                _outputCacheStored = outputCacheStored;
            }

            public async Task<Response<FiscalYearDto>> Handle(CreateFiscalYearCommand request, CancellationToken cancellationToken)
            {
                var existing = await _repositoryAsync.FirstOrDefaultAsync(new FilterFiscalYearByYearSpecification(request.Year), cancellationToken);
                if (existing != null)
                    throw new ApiException($"Ya existe el ejercicio fiscal {request.Year}.");

                var userName = _jwtService.GetSubjectToken();
                var culture = new CultureInfo("es-HN");

                var fiscalYear = new FiscalYear
                {
                    Year = request.Year,
                    Name = $"Ejercicio {request.Year}",
                    StartDate = new DateTime(request.Year, 1, 1),
                    EndDate = new DateTime(request.Year, 12, 31),
                    Status = FiscalPeriodStatus.Open,
                    IsClosed = false,
                    CreationDate = DateTime.Now,
                    CreatedBy = userName,
                    Periods = new List<FiscalPeriod>()
                };

                for (var month = 1; month <= 12; month++)
                {
                    var start = new DateTime(request.Year, month, 1);
                    var monthName = culture.TextInfo.ToTitleCase(start.ToString("MMMM", culture));
                    fiscalYear.Periods.Add(new FiscalPeriod
                    {
                        PeriodNumber = month,
                        Name = $"{monthName} {request.Year}",
                        StartDate = start,
                        EndDate = start.AddMonths(1).AddDays(-1),
                        Status = FiscalPeriodStatus.Open,
                        CreationDate = DateTime.Now,
                        CreatedBy = userName
                    });
                }

                var data = await _repositoryAsync.AddAsync(fiscalYear, cancellationToken);
                await _outputCacheStored.EvictByTagAsync("cache_fiscal_periods", cancellationToken);

                var full = await _repositoryAsync.FirstOrDefaultAsync(new FilterFiscalYearByIdSpecification(data.Id), cancellationToken);
                var dto = _mapper.Map<FiscalYearDto>(full);
                return new Response<FiscalYearDto>(dto, $"Ejercicio fiscal {request.Year} creado con sus 12 períodos.");
            }
        }
    }
}
