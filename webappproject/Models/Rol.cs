using System.ComponentModel.DataAnnotations;

namespace webappproject.Models
{
    public class Rol: BaseEntity
    {
        public string Name { get; set; } = string.Empty;
    }
}
