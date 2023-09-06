using System.ComponentModel.DataAnnotations;

namespace webappproject.Models
{
    public class User : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ProfileUrl { get; set;} = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string ImagePath2 { get; set; } = string.Empty;
        public string Biography { get; set; } = string.Empty;
        public string Tag1 { get; set; } = string.Empty;
        public string Tag2 { get; set; } = string.Empty;
        public bool FirstLogIn { get; set; }
        public int SliderValue1 { get; set; }
        public int SliderValue2 { get; set; }
        public int Age { get; set; }
        public int RolId { get; set; }
    }
}
