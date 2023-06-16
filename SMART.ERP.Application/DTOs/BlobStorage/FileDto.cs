using Microsoft.AspNetCore.Http;

namespace SMART.ERP.Application.DTOs.BlobStorage
{
    public class FileDto
    {
        public IFormFile File { get; set; } = null!;
    }
}
