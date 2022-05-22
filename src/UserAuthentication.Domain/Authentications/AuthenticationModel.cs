using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserAuthentication.Domain.Authentications
{
    [Table("Authentication")]
    public class AuthenticationModel : IAggregateRoot
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string Mobile { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
