using back_end.Domain;

namespace back_end.Infraestructure
{
    public interface ICashUnitRepository
    {
        public List<CashUnit> GetAll();
        public void UpdateQuantity(int value, int modification);
    }
}