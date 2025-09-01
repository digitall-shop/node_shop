using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;

namespace Domain.Entities;

[Table("BankAccounts", Schema = "Account")]
public class BankAccount : BaseEntity<long>
{
    public string BankName { get; set; }
    public string HolderName { get; set; }
    public string CardNumber { get; set; } 
    public bool IsActive { get; set; }

    public ICollection<PaymentRequest> PaymentRequests { get; set; }
}