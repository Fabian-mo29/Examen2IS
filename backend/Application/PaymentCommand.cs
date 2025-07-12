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

        public List<CashUnit> Create(Payment payment)
        {
            try {
                var validCashUnits = this._cashUnitRepository.GetAll();
                if (!areCashUnitsValid(payment, validCashUnits))
                {
                    throw new InvalidOperationException("Invalid cash units.");
                }
                var availableSodas = this._sodaRepository.GetAll();
                if (!areSodaAvailable(availableSodas, payment))
                {
                    throw new InvalidOperationException("Sodas not available.");
                }
                //Aqui deberia revisar el vuelto
                var change = new List<CashUnit>();
                updateStocks(payment);
                return change;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Payment processing failed.");
            }
        }

        private bool areCashUnitsValid(Payment payment, List<CashUnit> validCashUnits)
        {
            if (payment == null || validCashUnits == null || validCashUnits.Count == 0)
            {
                return false;
            }

            foreach (var cashUnit in payment.cashUnits)
            {
                if (!validCashUnits.Any(c => c.value == cashUnit.value) || cashUnit.quantity <= 0)
                {
                    return false;
                }
            }
            return true;
        }

        private bool areSodaAvailable(List<Soda> availableSodas, Payment payment)
        {
            if (availableSodas == null || payment?.sodas == null)
                return false;

            foreach (var soda in payment.sodas)
            {
                if (!availableSodas.Any(s => s.name == soda.name))
                {
                    return false;
                }
            }
            return true;
        }

        private void updateStocks(Payment payment)
        {
            foreach (var cashUnit in payment.cashUnits)
            {
                this._cashUnitRepository.UpdateQuantity(cashUnit.value, cashUnit.quantity);
            }
            foreach (var soda in payment.sodas)
            {
                this._sodaRepository.DecreaseQuantity(soda.name, soda.quantity);
            }
        }

    }
}