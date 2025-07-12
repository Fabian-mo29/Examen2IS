using System.ComponentModel.DataAnnotations;

namespace back_end.Domain
{
    public class Soda
    {
        [Required]
        public String name { get; set; }
    
        public int? price { get; set; }

        [Required]
        public int quantity { get; set; }
    }
}