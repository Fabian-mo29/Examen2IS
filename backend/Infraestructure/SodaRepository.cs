using back_end.Domain;

namespace back_end.Infraestructure
{
    public class SodaRepository : ISodaRepository
    {
        private List<Soda> _sodas;

        public SodaRepository()
        {
            this._sodas = new List<Soda>
            {
                new Soda { name = "Coca Cola", price = 800, stock = 10 },
                new Soda { name = "Pepsi", price = 750, stock = 8 },
                new Soda { name = "Fanta", price = 950, stock = 10 },
                new Soda { name = "Sprite", price = 975, stock = 15 },
            };
        }

        public List<Soda> GetAllSodas()
        {
            return this._sodas;
        }

        public void DecreaseStock(string sodaName, int quantity)
        {
            try {
                var soda = this._sodas.FirstOrDefault(s => s.name == sodaName);
                if (soda != null)
                {
                    if (soda.stock < quantity)
                    {
                        throw new InvalidOperationException($"Not enough stock available for {sodaName}.");
                    }
                    soda.stock -= quantity;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while decreasing soda stock.");
            }
        }

    }
}
