using back_end.Domain;

namespace back_end.Infraestructure
{
    public interface ICashUnitRepository
    {
        public List<CashUnit> GetAllCashUnits();
        public void UpdateStock(int value, int modification);
    }
}