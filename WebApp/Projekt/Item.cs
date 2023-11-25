using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Projekt
{
    public class Item
    {
        [Required]
        public String Name { get; set; }
        [Required]
        public String Description { get; set; }
        [Required]
        public Double Price { get; set; }
        [Required]
        public String State { get; set; }
        [Required]
        public String Category { get; set; }
        
        public String ImageData { get; set; }
        [Key]
        public int Id { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
