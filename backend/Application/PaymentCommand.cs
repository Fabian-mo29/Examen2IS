using back_end.Domain;
using back_end.Infraestructure;

namespace back_end.Application
{
    public class PaymentCommand : IPaymentCommand
    {
        private readonly ICashUnitRepository _cashUnitRepository;
        private readonly ISodaRepository _sodaRepository;

        public PaymentCommand(ICashUnitRepository cashUnitRepository, ISodaRepository sodaRepository)
        {
            _cashUnitRepository = cashUnitRepository;
            _sodaRepository = sodaRepository;
        }

        public List<CashUnit> Create()
        {
            throw new NotImplementedException("Payment creation logic is not implemented yet.");
        }

    }
}