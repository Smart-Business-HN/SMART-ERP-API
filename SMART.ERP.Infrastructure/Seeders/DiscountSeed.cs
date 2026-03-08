using Microsoft.EntityFrameworkCore;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Infrastructure.Seeders
{
    public static class DiscountSeed
    {
        public static async Task SeedAsync(DatabaseContext context)
        {
            if (!await context.Discounts.AnyAsync(x => x.Id == 1))
            {
                context.Discounts.Add(new Discount { Name = "Ninguno", Rate = 0 });
                await context.SaveChangesAsync();
            }
        }
    }
}
