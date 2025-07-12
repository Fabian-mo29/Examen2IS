using System.ComponentModel.DataAnnotations;

namespace back_end.Domain
{
    public class Payment
    {
        [Required]
        public List<CashUnit> CashUnits { get; set; }

        [Required]
        public List<Soda> Sodas { get; set; }
    }
}