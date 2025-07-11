using System.ComponentModel.DataAnnotations;

namespace back_end.Domain
{
    public class Soda
    {
        [Required]
        public String name { get; set; }
    
        [Required]
        public int price { get; set; }

        [Required]
        public int stock { get; set; }
    }
}