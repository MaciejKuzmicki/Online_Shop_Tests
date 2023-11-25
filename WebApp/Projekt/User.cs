using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt
{
    public class User
    {
        [Required]
        public String Name { get; set; }
        [Required]
        public String Email { get; set; }
        [Required, PasswordPropertyText]
        public String Password { get; set; }
        [Required]
        public String Surname { get; set; }
        [Required]
        public String Role { get; set; }
        [Required]
        public String ObservedCategory { get; set; }
        [Required]
        public String PhoneNumber { get; set; }
        [Key]
        public int Id { get; set; }
        [Required(AllowEmptyStrings =true)]
        public String Items { get; set; }
    }
}
