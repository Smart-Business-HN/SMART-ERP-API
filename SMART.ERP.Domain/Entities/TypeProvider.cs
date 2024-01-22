using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Domain.Entities
{
    public class TypeProvider
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
    }
}
