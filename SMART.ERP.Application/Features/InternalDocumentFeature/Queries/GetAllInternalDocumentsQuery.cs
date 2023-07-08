using MediatR;
using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Features.InternalDocumentFeature.Queries
{
    public class GetAllInternalDocumentsQuery : IRequest<PagedResponse<List<InternalDocumentDto>>>
    {
        public string? Parameter { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Order { get; set; }
        public string? Column { get; set; }
        public bool All { get; set; }
    }
}
