using back_end.Domain;

namespace back_end.Application
{
    public interface ISodaQuery
    {
        public List<Soda> GetAllSodas();
    }
}
