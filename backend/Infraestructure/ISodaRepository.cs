using back_end.Domain;

namespace back_end.Infraestructure
{
    public interface ISodaRepository
    {
        public List<Soda> GetAllSodas();
    }
}