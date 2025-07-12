using System.ComponentModel.DataAnnotations;

namespace back_end.Domain
{
    public class Payment
    {
        [Required]
        public List<CashUnit> cashUnits { get; set; }

        [Required]
        public List<Soda> sodas { get; set; }
    }
}