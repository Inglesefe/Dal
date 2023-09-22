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
            //Arrange
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
            //Act
            ListResult<PaymentType> list = _persistent.List("idpaymenttype = 1", "name", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de tipos de pago con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.List("idtipopago = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un tipo de pago dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            PaymentType paymentType = new() { Id = 1 };

            //Act
            paymentType = _persistent.Read(paymentType);

            //Assert
            Assert.Equal("Efectivo", paymentType.Name);
        }

        /// <summary>
        /// Prueba la consulta de un tipo de pago que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            PaymentType paymentType = new() { Id = 10 };

            //Act
            paymentType = _persistent.Read(paymentType);

            //Assert
            Assert.Equal(0, paymentType.Id);
        }

        /// <summary>
        /// Prueba la consulta de un tipo de pago con error
        /// </summary>
        [Fact]
        public void ReadWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Read(null));
        }

        /// <summary>
        /// Prueba la inserción de un tipo de pago
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            PaymentType paymentType = new() { Name = "Daviplata" };

            //Act
            paymentType = _persistent.Insert(paymentType, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, paymentType.Id);
        }

        /// <summary>
        /// Prueba la inserción de un tipo de pago con error
        /// </summary>
        [Fact]
        public void InsertWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Insert(null, new() { Id = 0 }));
        }

        /// <summary>
        /// Prueba la actualización de un tipo de pago
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            PaymentType paymentType = new() { Id = 2, Name = "Visa" };
            PaymentType paymentType2 = new() { Id = 2 };

            //Act
            _ = _persistent.Update(paymentType, new() { Id = 1 });
            paymentType2 = _persistent.Read(paymentType2);

            //Assert
            Assert.NotEqual("Nequi", paymentType2.Name);
        }

        /// <summary>
        /// Prueba la actualización de un tipo de pago con error
        /// </summary>
        [Fact]
        public void UpdateWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Update(null, new() { Id = 0 }));
        }

        /// <summary>
        /// Prueba la eliminación de un tipo de pago
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            PaymentType paymentType = new() { Id = 3 };
            PaymentType paymentType2 = new() { Id = 3 };

            //Act
            _ = _persistent.Delete(paymentType, new() { Id = 1 });
            paymentType2 = _persistent.Read(paymentType2);

            //Assert
            Assert.Equal(0, paymentType2.Id);
        }

        /// <summary>
        /// Prueba la eliminación de un tipo de pago con error
        /// </summary>
        [Fact]
        public void DeleteWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Delete(null, new() { Id = 0 }));
        }
        #endregion
    }
}
