using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities;

public class LogEcommerceUser
{
    public int Id { get; init; }
    public Guid EcommerceUserId { get; set; }
    public virtual EcommerceUser? EcommerceUser { get; set; }
    public int ActionType { get; set; }
    [MaxLength(500)]
    public string? Details { get; set; }
    public DateTime CreationDate { get; set; }
}
