using System.ComponentModel.DataAnnotations;

namespace webappproject.Models
{
    public class BannedUser : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime BanDate { get; set; } = DateTime.Now;
        public string BannedBy { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}
