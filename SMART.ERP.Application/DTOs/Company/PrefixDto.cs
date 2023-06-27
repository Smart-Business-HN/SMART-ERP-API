using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.DTOs.Company
{
    public class PrefixDto
    {
        public int Id { get; init; }
        public string Prefix { get; set; } = null!;
        public int InternalDocumentId { get; set; }
        public virtual InternalDocument InternalDocument { get; set; } = null!;
        public bool ItIsTaken { get; set; }
    }
}
