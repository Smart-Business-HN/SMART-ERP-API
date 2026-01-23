namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class SalesPipelineDto
    {
        public List<PipelineStageDto> Stages { get; set; } = new();
        public decimal TotalPipelineValue { get; set; }
        public decimal WeightedPipelineValue { get; set; }
        public int TotalOpportunities { get; set; }
    }

    public class PipelineStageDto
    {
        public int StepId { get; set; }
        public string StepName { get; set; } = string.Empty;
        public int OpportunityCount { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal WeightedValue { get; set; }
        public decimal AverageProbability { get; set; }
    }
}
