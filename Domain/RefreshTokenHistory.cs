using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class RefreshTokenHistory
{
    [Key]
    public Guid? IdTokenHistory { get; set; }

    [ForeignKey("IdUser")]
    public required Guid IdUser { get; set; }

    public required string Token { get; set; }

    public required string RefreshToken { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    [NotMapped]
    public bool Active
    {
        get { return DateTime.UtcNow < ExpirationDate; }
        private set { }
    }
    public virtual User? User { get; set; }
}
