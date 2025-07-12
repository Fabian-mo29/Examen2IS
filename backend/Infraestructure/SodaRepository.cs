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
                new Soda { name = "Coca Cola", price = 800, quantity = 10 },
                new Soda { name = "Pepsi", price = 750, quantity = 8 },
                new Soda { name = "Fanta", price = 950, quantity = 10 },
                new Soda { name = "Sprite", price = 975, quantity = 15 },
            };
        }

        public List<Soda> GetAll()
        {
            return this._sodas;
        }

        public void DecreaseQuantity(string sodaName, int quantity)
        {
            try {
                var soda = this._sodas.FirstOrDefault(s => s.name == sodaName);
                if (soda != null)
                {
                    if (soda.quantity < quantity)
                    {
                        throw new InvalidOperationException($"Not enough quantity available for {sodaName}.");
                    }
                    soda.quantity -= quantity;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while decreasing soda quantity.");
            }
        }

    }
}
