using System.ComponentModel.DataAnnotations;

namespace back_end.Domain
{
    public class CashUnit
    {
        [Required]
        public int value { get; set; }

        [Required]
        public int quantity { get; set; }
    }
}