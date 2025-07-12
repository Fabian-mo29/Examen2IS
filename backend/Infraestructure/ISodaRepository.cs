using back_end.Domain;

namespace back_end.Infraestructure
{
    public interface ISodaRepository
    {
        public List<Soda> GetAll();
        public void DecreaseQuantity(string sodaName, int quantity);
    }
}