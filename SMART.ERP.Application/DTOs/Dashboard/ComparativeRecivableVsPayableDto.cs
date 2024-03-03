using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class ComparativeRecivableVsPayableDto
    {
        public Decimal Payable {  get; set; }
        public Decimal Recivable { get; set; }
    }
}
