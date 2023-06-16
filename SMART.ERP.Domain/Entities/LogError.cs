namespace SMART.ERP.Domain.Entities
{
    public class LogError
    {
        public int Id { get; init; }
        public string Message { get; set; } = null!;
        public string StackTrace { get; set; } = null!;
        public DateTime CreationDate { get; set; }
    }
}
