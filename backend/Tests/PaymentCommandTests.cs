using back_end.Application;
using back_end.Infraestructure;
using back_end.Domain;
using Moq;

namespace Tests
{
    public class PaymentCommandTests
    {
        private PaymentCommand _paymentCommand;
        private Mock<ISodaRepository> _sodaRepository;
        private Mock<ICashUnitRepository> _cashUnitRepository;


        [SetUp]
        public void Setup()
        {
            this._cashUnitRepository = new Mock<ICashUnitRepository>();
            this._sodaRepository = new Mock<ISodaRepository>();
            this._paymentCommand = new PaymentCommand(_cashUnitRepository.Object, _sodaRepository.Object);
        }

        [Test]
        public void Create_ShouldThrow_WhenSodaNotValid()
        {
            var payment = new Payment
            {
                sodas = new List<Soda> { new Soda { name = "7Up", quantity = 1 } },
                cashUnits = new List<CashUnit> { new CashUnit { value = 1000, quantity = 1 } }
            };

            // Ejemplo cualquiera del stock
            _sodaRepository.Setup(r => r.GetAll()).Returns(new List<Soda> {
                new Soda { name = "Coca Cola", quantity = 5, price = 800 }
            });

            // Ejemplo cualquiera de efectivo disponible
            _cashUnitRepository.Setup(r => r.GetAll()).Returns(new List<CashUnit> {
                new CashUnit { value = 1000, quantity = 30 }
            });

            var ex = Assert.Throws<InvalidOperationException>(() => _paymentCommand.Create(payment));
            Assert.That(ex.Message, Does.Contain("Requested sodas are not available."));
        }

        [Test]
        public void Create_ShouldThrow_WhenCashUnitNotValid()
        {
            // Se agrega un tipo de pago no valido
            var payment = new Payment
            {
                sodas = new List<Soda> { new Soda { name = "Fanta", quantity = 1 } },
                cashUnits = new List<CashUnit> { new CashUnit { value = 999, quantity = 2 } }
            };

            _sodaRepository.Setup(r => r.GetAll()).Returns(new List<Soda> {
                new Soda { name = "Fanta", quantity = 8, price = 950 }
            });

            // Ejemplo cualquiera de efectivo disponible
            _cashUnitRepository.Setup(r => r.GetAll()).Returns(new List<CashUnit> {
                new CashUnit { value = 1000, quantity = 0 },
                new CashUnit { value = 500, quantity = 10 },
                new CashUnit { value = 100, quantity = 10 },
                new CashUnit { value = 50, quantity = 10 },
                new CashUnit { value = 25, quantity = 10 }
            });

            var ex = Assert.Throws<InvalidOperationException>(() => _paymentCommand.Create(payment));
            Assert.That(ex.Message, Does.Contain("Invalid cash units in payment."));
        }

        [Test]
        public void Create_ShouldThrow_WhenSodaQuantityIsGreaterThanStock()
        {
            // Se agrega una cantidad de soda superior al stock actual
            var payment = new Payment
            {
                sodas = new List<Soda> { new Soda { name = "Fanta", quantity = 4 } },
                cashUnits = new List<CashUnit> { new CashUnit { value = 1000, quantity = 4 } }
            };

            _sodaRepository.Setup(r => r.GetAll()).Returns(new List<Soda> {
                new Soda { name = "Fanta", quantity = 3, price = 950 }
            });

            // Ejemplo cualquiera de efectivo disponible
            _cashUnitRepository.Setup(r => r.GetAll()).Returns(new List<CashUnit> {
                new CashUnit { value = 1000, quantity = 0 },
                new CashUnit { value = 500, quantity = 10 },
                new CashUnit { value = 100, quantity = 10 },
                new CashUnit { value = 50, quantity = 10 },
                new CashUnit { value = 25, quantity = 10 }
            });

            var ex = Assert.Throws<InvalidOperationException>(() => _paymentCommand.Create(payment));
            Assert.That(ex.Message, Does.Contain("Requested quantity for Fanta is greater than the current stocks."));
        }

        [Test]
        public void Create_ShouldThrow_WhenCashUnitStockIsNotEnoughForChange()
        {
            var payment = new Payment
            {
                sodas = new List<Soda> { new Soda { name = "Sprite", quantity = 1 } },
                cashUnits = new List<CashUnit> { new CashUnit { value = 1000, quantity = 1 } }
            };

            _sodaRepository.Setup(r => r.GetAll()).Returns(new List<Soda> {
                new Soda { name = "Sprite", quantity = 3, price = 975 }
            });

            // Ejemplo donde no hay monedas disponibles para el cambio
            _cashUnitRepository.Setup(r => r.GetAll()).Returns(new List<CashUnit> {
                new CashUnit { value = 1000, quantity = 0 },
                new CashUnit { value = 500, quantity = 10 },
                new CashUnit { value = 100, quantity = 10 },
                new CashUnit { value = 50, quantity = 10 },
                new CashUnit { value = 25, quantity = 0 }
            });

            var ex = Assert.Throws<InvalidOperationException>(() => _paymentCommand.Create(payment));
            Assert.That(ex.Message, Does.Contain("There is no cash available for change."));
        }

        [Test]
        public void Create_ShouldReturnEmptyChange_WhenPayedAmountIsExact()
        {
            var payment = new Payment
            {
                sodas = new List<Soda> {
                    new Soda { name = "Fanta", quantity = 1 }
                },
                cashUnits = new List<CashUnit> {
                    new CashUnit { value = 500, quantity = 1 },
                    new CashUnit { value = 100, quantity = 4 },
                    new CashUnit { value = 50, quantity = 1 }
                }
            };

            _sodaRepository.Setup(r => r.GetAll()).Returns(new List<Soda> {
                new Soda { name = "Fanta", quantity = 5, price = 950 }
            });

            _cashUnitRepository.Setup(r => r.GetAll()).Returns(new List<CashUnit> {
                new CashUnit { value = 1000, quantity = 5 },
                new CashUnit { value = 500, quantity = 5 },
                new CashUnit { value = 100, quantity = 5 },
                new CashUnit { value = 50, quantity = 5 },
                new CashUnit { value = 25, quantity = 5 }
            });

            var result = _paymentCommand.Create(payment);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Create_ShouldThrow_WhenPayedAmountIsNotEnough()
        {
            // No se paga lo suficiente
            var payment = new Payment
            {
                sodas = new List<Soda> { new Soda { name = "Sprite", quantity = 2 } },
                cashUnits = new List<CashUnit> { new CashUnit { value = 50, quantity = 1 } }
            };

            _sodaRepository.Setup(r => r.GetAll()).Returns(new List<Soda> {
                new Soda { name = "Sprite", quantity = 3, price = 975 }
            });

            _cashUnitRepository.Setup(r => r.GetAll()).Returns(new List<CashUnit> {
                new CashUnit { value = 1000, quantity = 0 },
                new CashUnit { value = 500, quantity = 10 },
                new CashUnit { value = 100, quantity = 10 },
                new CashUnit { value = 50, quantity = 10 },
                new CashUnit { value = 25, quantity = 10 }
            });

            var ex = Assert.Throws<InvalidOperationException>(() => _paymentCommand.Create(payment));
            Assert.That(ex.Message, Does.Contain("Insufficient amount paid."));
        }

        [Test]
        public void Create_ShouldReturnTheMoneyBack_WhenPayedAmountIsExcesive()
        {
            var payment = new Payment
            {
                sodas = new List<Soda> {
                    new Soda { name = "Fanta", quantity = 1 }
                },
                cashUnits = new List<CashUnit> {
                    new CashUnit { value = 500, quantity = 5 },
                    new CashUnit { value = 1000, quantity = 4 }
                }
            };

            _sodaRepository.Setup(r => r.GetAll()).Returns(new List<Soda> {
                new Soda { name = "Fanta", quantity = 5, price = 950 }
            });

            _cashUnitRepository.Setup(r => r.GetAll()).Returns(new List<CashUnit> {
                new CashUnit { value = 1000, quantity = 5 },
                new CashUnit { value = 500, quantity = 5 },
                new CashUnit { value = 100, quantity = 5 },
                new CashUnit { value = 50, quantity = 5 },
                new CashUnit { value = 25, quantity = 5 }
            });

            var result = _paymentCommand.Create(payment);

            var expectedChange = new List<CashUnit>
            {
                new CashUnit { value = 1000, quantity = 5 },
                new CashUnit { value = 500, quantity = 1 },
                new CashUnit { value = 50, quantity = 1 }
            };

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(expectedChange.Count));
            foreach (var expected in expectedChange)
            {
                var actual = result.FirstOrDefault(c => c.value == expected.value);
                Assert.That(actual, Is.Not.Null, $"Expected denomination {expected.value} not found.");
                Assert.That(actual.quantity, Is.EqualTo(expected.quantity), $"Mismatch in quantity for {expected.value}");
            }
        }

        [Test]
        public void Create_ShouldReturnChange_WhenPayedDecentAmount()
        {
            var payment = new Payment
            {
                sodas = new List<Soda> {
                    new Soda { name = "Coca Cola", quantity = 2 },
                    new Soda { name = "Sprite", quantity = 1 }
                },
                cashUnits = new List<CashUnit> {
                    new CashUnit { value = 1000, quantity = 2 },
                    new CashUnit { value = 500, quantity = 2 }
                }
            };

            _sodaRepository.Setup(r => r.GetAll()).Returns(new List<Soda> {
                new Soda { name = "Coca Cola", price = 800, quantity = 10 },
                new Soda { name = "Pepsi", price = 750, quantity = 8 },
                new Soda { name = "Fanta", price = 950, quantity = 10 },
                new Soda { name = "Sprite", price = 975, quantity = 15 }
            });

            _cashUnitRepository.Setup(r => r.GetAll()).Returns(new List<CashUnit> {
                new CashUnit { value = 1000, quantity = 0 },
                new CashUnit { value = 500, quantity = 20 },
                new CashUnit { value = 100, quantity = 30 },
                new CashUnit { value = 50, quantity = 50 },
                new CashUnit { value = 25, quantity = 25 }
            });

            var result = _paymentCommand.Create(payment);

            var expectedChange = new List<CashUnit>
            {
                new CashUnit { value = 100, quantity = 4 },
                new CashUnit { value = 25, quantity = 1 }
            };

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(expectedChange.Count));
            foreach (var expected in expectedChange)
            {
                var actual = result.FirstOrDefault(c => c.value == expected.value);
                Assert.That(actual, Is.Not.Null, $"Expected denomination {expected.value} not found.");
                Assert.That(actual.quantity, Is.EqualTo(expected.quantity), $"Mismatch in quantity for {expected.value}");
            }
        }

    }
}