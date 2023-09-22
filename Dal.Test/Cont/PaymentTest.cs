using Dal.Cont;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Cont;
using Microsoft.Extensions.Configuration;

namespace Dal.Test.Cont
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de pagos
    /// </summary>
    [Collection("Tests")]
    public class PaymentTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de los pagos
        /// </summary>
        private readonly PersistentPayment _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public PaymentTest()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .AddEnvironmentVariables()
                .Build();
            _persistent = new(_configuration.GetConnectionString("golden") ?? "");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de pagos con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            ListResult<Payment> list = _persistent.List("idpayment = 1", "value", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de pagos con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idpago = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un pago dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            Payment payment = new() { Id = 1 };
            payment = _persistent.Read(payment);

            Assert.Equal(1500, payment.Value);
        }

        /// <summary>
        /// Prueba la consulta de un pago que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            Payment payment = new() { Id = 10 };
            payment = _persistent.Read(payment);

            Assert.Equal(0, payment.Id);
        }

        /// <summary>
        /// Prueba la inserción de un pago
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            Payment payment = new() { PaymentType = new() { Id = 1 }, Fee = new() { Id = 1 }, Value = 4500, Date = DateTime.Now, Invoice = "101-000004", Proof = "http://localhost/prueba4.png" };
            payment = _persistent.Insert(payment, new() { Id = 1 });

            Assert.NotEqual(0, payment.Id);
        }

        /// <summary>
        /// Prueba la actualización de un pago
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            Payment payment = new() { Id = 2, PaymentType = new() { Id = 1 }, Fee = new() { Id = 1 }, Value = 5500, Date = DateTime.Now, Invoice = "101-000005", Proof = "http://localhost/prueba5.png" };
            _ = _persistent.Update(payment, new() { Id = 1 });

            Payment payment2 = new() { Id = 2 };
            payment2 = _persistent.Read(payment2);

            Assert.NotEqual(2500, payment2.Value);
        }

        /// <summary>
        /// Prueba la eliminación de un pago
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            Payment payment = new() { Id = 3 };
            _ = _persistent.Delete(payment, new() { Id = 1 });

            Payment payment2 = new() { Id = 3 };
            payment2 = _persistent.Read(payment2);

            Assert.Equal(0, payment2.Id);
        }
        #endregion
    }
}