using MediatR;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Specifications.OpportunitySpecification;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.DTOs.Dashboard;

namespace SMART.ERP.Application.Features.DashboardFeature.Queries.OpportunityMetrics
{
    public class WinAndLossMetricQuery : IRequest<Response<List<WinAndLossMetricDto>>>
    {
        public int BranchOfficeId { get; set; }
    }
    public class WinAndLossMetricQueryHandler : IRequestHandler<WinAndLossMetricQuery, Response<List<WinAndLossMetricDto>>>
    {
        private readonly IRepositoryAsync<Opportunity> _repositoryAsync;

        public WinAndLossMetricQueryHandler(IRepositoryAsync<Opportunity> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<List<WinAndLossMetricDto>>> Handle(WinAndLossMetricQuery request, CancellationToken cancellationToken)
        {
            List<WinAndLossMetricDto> result = new List<WinAndLossMetricDto>();
            var getWinOpportunity = await _repositoryAsync.ListAsync(new WinAndLossOpportunitiesSpecification("Win", false, false, request.BranchOfficeId));
            if (getWinOpportunity.Count > 0)
            {
                WinAndLossMetricDto win = new WinAndLossMetricDto();
                win.MetricName = "Win";
                win.Quantity = getWinOpportunity.Count;
                foreach (var item in getWinOpportunity)
                {
                    if (item.QuoteProducts != null)
                    {
                        win.Current += item.QuoteProducts!.Sum(a => a.SalePrice * a.Quantity);
                    }
                }
                var compare = await _repositoryAsync.ListAsync(new WinAndLossOpportunitiesSpecification("Win", true, false, request.BranchOfficeId));
                if (compare.Count > 0)
                {
                    foreach (var item in compare)
                    {
                        if (item.QuoteProducts != null)
                        {
                            win.Previous += item.QuoteProducts!.Sum(a => a.SalePrice * a.Quantity);
                        }
                    }
                    win.Difference = win.Current - win.Previous;
                    win.Percentage = win.Difference * 100 / win.Previous;
                }
                else
                {
                    win.Percentage = 100;
                }
                result.Add(win);
            }
            else
            {
                WinAndLossMetricDto win = new WinAndLossMetricDto();
                win.MetricName = "Win";
                win.Quantity = 0;
                win.Current = 0;
                win.Previous = 0;
                win.Difference = 0;
                win.Percentage = 0;
                result.Add(win);

            }

            var getLossOpportunity = await _repositoryAsync.ListAsync(new WinAndLossOpportunitiesSpecification("Loss", false, false, request.BranchOfficeId));
            if (getLossOpportunity.Count > 0)
            {
                WinAndLossMetricDto loss = new WinAndLossMetricDto();
                loss.MetricName = "Loss";
                loss.Quantity = getLossOpportunity.Count;
                foreach (var item in getLossOpportunity)
                {
                    if (item.QuoteProducts != null)
                    {
                        loss.Current += item.QuoteProducts!.Sum(a => a.SalePrice * a.Quantity);
                    }
                }
                var compare = await _repositoryAsync.ListAsync(new WinAndLossOpportunitiesSpecification("Loss", true, false, request.BranchOfficeId));
                if (compare.Count > 0)
                {
                    foreach (var item in compare)
                    {
                        if (item.QuoteProducts != null)
                        {
                            loss.Previous += item.QuoteProducts!.Sum(a => a.SalePrice * a.Quantity);
                        }
                    }
                    loss.Difference = loss.Current - loss.Previous;
                    loss.Percentage = loss.Difference * 100 / loss.Previous;
                }
                else
                {
                    loss.Percentage = 100;
                }
                result.Add(loss);
            }
            else
            {
                WinAndLossMetricDto loss = new WinAndLossMetricDto();
                loss.MetricName = "Loss";
                loss.Quantity = 0;
                loss.Current = 0;
                loss.Previous = 0;
                loss.Difference = 0;
                loss.Percentage = 0;
                result.Add(loss);
            }

            return new Response<List<WinAndLossMetricDto>>(result);
        }
    }
}
