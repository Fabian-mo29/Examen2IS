using back_end.Domain;

namespace back_end.Infraestructure
{
    public class CashUnitRepository : ICashUnitRepository
    {
        private List<CashUnit> _cashUnits;
        
        public CashUnitRepository()
        {
            this._cashUnits = new List<CashUnit>(){
                new CashUnit { value = 1000, quantity = 0 },
                new CashUnit { value = 500, quantity = 20 },
                new CashUnit { value = 100, quantity = 30 },
                new CashUnit { value = 50, quantity = 50 },
                new CashUnit { value = 25, quantity = 25 }
            };
        }

        public List<CashUnit> GetAll()
        {
            return _cashUnits;
        }

        public void UpdateQuantity(int value, int modification)
        {
            var cashUnit = _cashUnits.FirstOrDefault(cu => cu.value == value);
            if (cashUnit != null)
            {
                if (cashUnit.quantity + modification < 0)
                {
                    throw new InvalidOperationException($"Insufficient quantity for {cashUnit.value}.");
                }
                cashUnit.quantity += modification;
            }
        }

       
    }
}