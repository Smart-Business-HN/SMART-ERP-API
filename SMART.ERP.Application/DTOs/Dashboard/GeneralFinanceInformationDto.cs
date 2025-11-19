

namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class GeneralFinanceInformationDto
    {
        public decimal AnnualSales { get; set; }
        public decimal AnnualExpenses { get; set; }
        public decimal CurrentMonthSales { get; set; }
        public decimal CurrentMonthExpenses { get; set; }
        public decimal PreviousMonthSales { get; set; }
        public decimal PreviousMonthExpenses { get; set; }
        public BrandSalesDto BrandSales { get; set; } = new BrandSalesDto();
        public CategorySalesDto CategorySales { get; set; } = new CategorySalesDto();
        public ExpensesDto Expenses { get; set; } = new ExpensesDto();
        public decimal Receivable { get; set; }
        public decimal Payable { get; set; }
        public decimal AcidTest { get; set; }
        public decimal AnnualGoal { get; set; }
        public decimal RealInventory { get; set; }
        public decimal AccountantInventory { get; set; }
    }
}
