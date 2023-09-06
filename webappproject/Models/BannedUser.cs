using System.ComponentModel.DataAnnotations;

namespace webappproject.Models
{
    public class BannedUser : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
    }
}
