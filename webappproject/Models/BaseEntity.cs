using System.ComponentModel.DataAnnotations;

namespace webappproject.Models
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
