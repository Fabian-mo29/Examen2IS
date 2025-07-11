using System.ComponentModel.DataAnnotations;

namespace back_end.Domain
{
    public class CashUnit
    {
        [Required]
        public int value { get; set; }

        [Required]
        public int stock { get; set; }
    }
}