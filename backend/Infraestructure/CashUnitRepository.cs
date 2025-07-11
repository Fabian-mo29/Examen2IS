using back_end.Domain;

namespace back_end.Infraestructure
{
    public class CashUnitRepository : ICashUnitRepository
    {
        private List<CashUnit> _cashUnits;
        
        public CashUnitRepository()
        {
            this._cashUnits = new List<CashUnit>(){
                new CashUnit { value = 1000, stock = 0 },
                new CashUnit { value = 500, stock = 20 },
                new CashUnit { value = 100, stock = 30 },
                new CashUnit { value = 50, stock = 50 },
                new CashUnit { value = 25, stock = 25 }
            };
        }

        public List<CashUnit> GetAllCashUnits()
        {
            return _cashUnits;
        }

        public void UpdateStock(int value, int modification)
        {
            var cashUnit = _cashUnits.FirstOrDefault(cu => cu.value == value);
            if (cashUnit != null)
            {
                if (cashUnit.stock + modification < 0)
                {
                    throw new InvalidOperationException($"Stock insufficient for {cashUnit.value}.");
                }
                cashUnit.stock += modification;
            }
        }

       
    }
}