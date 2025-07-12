using back_end.Domain;
using back_end.Infraestructure;

namespace back_end.Application
{
    public class SodaQuery : ISodaQuery
    {
        private readonly ISodaRepository _sodaRepository;

        public SodaQuery(ISodaRepository sodaRepository)
        {
            this._sodaRepository = sodaRepository;
        }

        public List<Soda> GetAll()
        {
            try {
                return this._sodaRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving sodas.");
            }
        }
    }
}