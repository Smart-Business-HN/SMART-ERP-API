using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.DTOs.Quotation
{
    public class KindOfTaxDto
    {
        public Tax Tax { get; set; } = null!;
        public decimal Amount { get; set; }

    }
}
