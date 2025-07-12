using back_end.Domain;

namespace back_end.Application
{
    public interface IPaymentCommand
    {
        public List<CashUnit> Create(Payment payment);
    }
}