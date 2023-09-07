using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.DTOs.InventoryInputType
{
    public class InventoryInputTypeDto
    {
        public int Id {  get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
