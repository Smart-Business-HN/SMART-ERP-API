using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Specifications.PrefixSpecification
{
    public class PrefixByInternalDocumentIdSpecification : Specification<Prefix>
    {
        public PrefixByInternalDocumentIdSpecification(int internalDocumentId)
        {
            Query.Include(x=>x.InternalDocument).Where(x=>x.InternalDocumentId == internalDocumentId);
        }
    }
}
