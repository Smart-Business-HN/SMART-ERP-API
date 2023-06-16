using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Dashboard;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.OpportunityMetrics
{
    public class ReasonWinAndLossOpportunityQuery : IRequest<Response<List<ReasonWinAndLossDto>>>
    {
        public int BranchOfficeId { get; set; }
    }

    public class ReasonWinAndLossOpportunityQueryHandler : IRequestHandler<ReasonWinAndLossOpportunityQuery, Response<List<ReasonWinAndLossDto>>>
    {
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;

        public ReasonWinAndLossOpportunityQueryHandler(IRepositoryAsync<Opportunity> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }
        public async Task<Response<List<ReasonWinAndLossDto>>> Handle(ReasonWinAndLossOpportunityQuery request, CancellationToken cancellationToken)
        {
            List<ReasonWinAndLossDto> result = new List<ReasonWinAndLossDto>();
            var getWinOpportunity = await _repositoryAsync.ListAsync(new WinAndLossOpportunitiesSpecification("Win", false, true, request.BranchOfficeId));
            if (getWinOpportunity.Count > 0)
            {
                ReasonWinAndLossDto win = new ReasonWinAndLossDto();
                win.MetricName = "Win";
                var groupByReason = getWinOpportunity
                    .GroupBy(a => a.WinReason!.Name)
                    .Select(a =>
                    new
                    {
                        Name = a.Key,
                        Total = a.Sum(a => a.Total),
                        Quantity = a.Count()
                    })
                    .MaxBy(a => a.Total);
                if (groupByReason != null)
                {
                    win.CurrentReasonName = groupByReason.Name;
                    win.Current = (decimal)groupByReason.Total;
                }
                var compare = await _repositoryAsync.ListAsync(new WinAndLossOpportunitiesSpecification("Win", true, true, request.BranchOfficeId));
                if (compare.Count > 0)
                {
                    var groupByReasonCompare = compare
                    .GroupBy(a => a.WinReason!.Name)
                    .Select(a =>
                    new
                    {
                        Name = a.Key,
                        Total = a.Sum(a => a.Total),
                        Quantity = a.Count()
                    })
                    .MaxBy(a => a.Total);
                    if (groupByReasonCompare != null)
                    {
                        win.PreviousReasonName = groupByReasonCompare.Name;
                        win.Previous = (decimal)groupByReasonCompare.Total;
                        if (win.Previous > 0)
                        {
                            win.Difference = win.Current - win.Previous;
                            win.Percentage = win.Difference * 100 / win.Previous;
                        }
                    }
                }
                else
                {
                    win.Percentage = 100;
                }
                result.Add(win);
            }
            else
            {
                ReasonWinAndLossDto win = new ReasonWinAndLossDto();
                win.MetricName = "Win";
                win.PreviousReasonName = "";
                win.CurrentReasonName = "";
                win.Current = 0;
                win.Previous = 0;
                win.Difference = 0;
                win.Percentage = 0;
                result.Add(win);

            }

            var getLossOpportunity = await _repositoryAsync.ListAsync(new WinAndLossOpportunitiesSpecification("Loss", false, true, request.BranchOfficeId));
            if (getLossOpportunity.Count > 0)
            {
                ReasonWinAndLossDto loss = new ReasonWinAndLossDto();
                loss.MetricName = "Loss";
                if (getLossOpportunity != null)
                {
                    var groupByReason = getLossOpportunity
                        .GroupBy(a => a.LossReason!.Name)
                        .Select(a =>
                        new
                        {
                            Name = a.Key,
                            Total = a.Sum(a => a.Total),
                            Quantity = a.Count()
                        })
                        .MaxBy(a => a.Total);
                    if (groupByReason != null)
                    {
                        loss.CurrentReasonName = groupByReason.Name;
                        loss.Current = (decimal)groupByReason.Total;
                    }
                    var compare = await _repositoryAsync.ListAsync(new WinAndLossOpportunitiesSpecification("Loss", true, true, request.BranchOfficeId));
                    if (compare.Count > 0)
                    {
                        var groupByReasonCompare = compare
                        .GroupBy(a => a.LossReason!.Name)
                        .Select(a =>
                        new
                        {
                            Name = a.Key,
                            Total = a.Sum(a => a.Total),
                            Quantity = a.Count()
                        })
                        .MaxBy(a => a.Total);
                        if (groupByReasonCompare != null)
                        {
                            loss.PreviousReasonName = groupByReasonCompare.Name;
                            loss.Previous = (decimal)groupByReasonCompare.Total;
                            if (loss.Previous > 0)
                            {
                                loss.Difference = loss.Current - loss.Previous;
                                loss.Percentage = loss.Difference * 100 / loss.Previous;
                            }
                        }
                    }
                    else
                    {
                        loss.Percentage = 100;
                    }
                }
                result.Add(loss);
            }
            else
            {
                ReasonWinAndLossDto loss = new ReasonWinAndLossDto();
                loss.MetricName = "Loss";
                loss.PreviousReasonName = "";
                loss.CurrentReasonName = "";
                loss.Current = 0;
                loss.Previous = 0;
                loss.Difference = 0;
                loss.Percentage = 0;
                result.Add(loss);
            }
            return new Response<List<ReasonWinAndLossDto>>(result);
        }
    }
}
