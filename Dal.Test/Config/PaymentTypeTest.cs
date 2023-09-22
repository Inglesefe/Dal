using Dal.Config;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Config;
using Microsoft.Extensions.Configuration;

namespace Dal.Test.Config
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de tipos de pago
    /// </summary>
    [Collection("Tests")]
    public class PaymentTypeTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de los tipos de pago
        /// </summary>
        private readonly PersistentPaymentType _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public PaymentTypeTest()
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
        /// Prueba la consulta de un listado de tipos de pago con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            ListResult<PaymentType> list = _persistent.List("idpaymenttype = 1", "name", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de tipos de pago con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idtipopago = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un tipo de pago dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            PaymentType paymentType = new() { Id = 1 };
            paymentType = _persistent.Read(paymentType);

            Assert.Equal("Efectivo", paymentType.Name);
        }

        /// <summary>
        /// Prueba la consulta de un tipo de pago que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            PaymentType paymentType = new() { Id = 10 };
            paymentType = _persistent.Read(paymentType);

            Assert.Equal(0, paymentType.Id);
        }

        /// <summary>
        /// Prueba la inserción de un tipo de pago
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            PaymentType paymentType = new() { Name = "Daviplata" };
            paymentType = _persistent.Insert(paymentType, new() { Id = 1 });

            Assert.NotEqual(0, paymentType.Id);
        }

        /// <summary>
        /// Prueba la actualización de un tipo de pago
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            PaymentType paymentType = new() { Id = 2, Name = "Visa" };
            _ = _persistent.Update(paymentType, new() { Id = 1 });

            PaymentType paymentType2 = new() { Id = 2 };
            paymentType2 = _persistent.Read(paymentType2);

            Assert.NotEqual("Nequi", paymentType2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de un tipo de pago
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            PaymentType paymentType = new() { Id = 3 };
            _ = _persistent.Delete(paymentType, new() { Id = 1 });

            PaymentType paymentType2 = new() { Id = 3 };
            paymentType2 = _persistent.Read(paymentType2);

            Assert.Equal(0, paymentType2.Id);
        }
        #endregion
    }
}
