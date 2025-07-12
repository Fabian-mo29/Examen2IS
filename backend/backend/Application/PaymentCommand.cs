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
            try
            {
                var validCashUnits = _cashUnitRepository.GetAll();
                var availableSodas = _sodaRepository.GetAll();
                validateInput(payment, validCashUnits, availableSodas);

                var allCashUnits = getCashUnitsWithPaymentAdded(payment, validCashUnits);

                int change = calculateChange(payment, allCashUnits, availableSodas);
                if (change < 0)
                {
                    throw new InvalidOperationException("Insufficient amount paid.");
                }

                var changeDistribution = getChangeDistribution(allCashUnits, change, payment.cashUnits);

                updateStocks(payment, changeDistribution);

                return changeDistribution;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Payment processing failed: {ex.Message}", ex);
            }
        }


        private void validateInput(Payment payment, List<CashUnit> validCashUnits, List<Soda> availableSodas)
        {
            if (payment == null)
            {
                throw new ArgumentNullException(nameof(payment), "Payment cannot be null.");
            }

            if (!areCashUnitsValid(payment, validCashUnits))
            {
                throw new InvalidOperationException("Invalid cash units in payment.");
            }

            if (!areSodaAvailable(payment, availableSodas))
            {
                throw new InvalidOperationException("Requested sodas are not available.");
            }
        }

        private bool areCashUnitsValid(Payment payment, List<CashUnit> validCashUnits)
        {
            if (validCashUnits == null || validCashUnits.Count == 0)
            {
               return false;
            }

            return payment.cashUnits.All(cu => validCashUnits.Any(vc => vc.value == cu.value) && cu.quantity > 0);
        }

        private bool areSodaAvailable(Payment payment, List<Soda> availableSodas)
        {
            if (availableSodas == null || payment?.sodas == null)
            {
                return false;
            }

            return payment.sodas.All(s => availableSodas.Any(av => av.name == s.name));
        }

        private int calculateChange(Payment payment, List<CashUnit> cashUnitsForChangeCalc, List<Soda> availableSodas)
        {
            int totalPrice = 0;
            foreach (var soda in payment.sodas)
            {
                var sodaPrice = availableSodas.FirstOrDefault(s => s.name == soda.name)?.price ?? 0;
                totalPrice += soda.quantity*sodaPrice;
            }
            int totalPayed = payment.cashUnits.Sum(cu => cu.value * cu.quantity);
            return totalPayed - totalPrice;
        }

        private List<CashUnit> getChangeDistribution(List<CashUnit> availableCashUnits, int change, List<CashUnit> insertedCashUnits)
        {
            var changeDistribution = new List<CashUnit>();

            foreach (var unit in availableCashUnits)
            {
                if (change <= 0) break;

                int numNeeded = change / unit.value;
                int numToUse = Math.Min(numNeeded, unit.quantity);

                if (numToUse > 0)
                {
                    changeDistribution.Add(new CashUnit
                    {
                        value = unit.value,
                        quantity = numToUse
                    });

                    change -= unit.value * numToUse;
                }
            }

            if (change > 0)
            {
                throw new InvalidOperationException("There is no cash available for change.");
            }

            return changeDistribution;
        }

        private List<CashUnit> getCashUnitsWithPaymentAdded(Payment payment, List<CashUnit> baseCashUnits)
        {
            var result = baseCashUnits
                .Select(cu => new CashUnit { value = cu.value, quantity = cu.quantity })
                .ToList();

            foreach (var paidUnit in payment.cashUnits)
            {
                var existingUnit = result.FirstOrDefault(cu => cu.value == paidUnit.value);
                if (existingUnit != null)
                {
                    existingUnit.quantity += paidUnit.quantity;
                }
                else
                {
                    result.Add(new CashUnit { value = paidUnit.value, quantity = paidUnit.quantity });
                }
            }

            return result;
        }

        private void updateStocks(Payment payment, List<CashUnit> changeDistribution)
        {
            // Se suma lo que el usuario acaba de ingresar
            try {

                foreach (var cashUnit in payment.cashUnits)
                {
                    _cashUnitRepository.UpdateQuantity(cashUnit.value, cashUnit.quantity);
                }

                // Se resta el cambio entregado
                foreach (var cashUnit in changeDistribution)
                {
                    _cashUnitRepository.UpdateQuantity(cashUnit.value, -cashUnit.quantity);
                }

                // Se actualiza el stock en refrescos
                foreach (var soda in payment.sodas)
                {
                    _sodaRepository.DecreaseQuantity(soda.name, soda.quantity);
                }
            }
            catch(Exception ex) {
                throw new InvalidOperationException("Unable to update the stock", ex);
            }
        }
    }
}
