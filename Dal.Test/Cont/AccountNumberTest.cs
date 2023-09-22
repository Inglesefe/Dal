using Dal.Cont;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Cont;
using Microsoft.Extensions.Configuration;

namespace Dal.Test.Cont
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de números de cuenta
    /// </summary>
    [Collection("Tests")]
    public class AccountNumberTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de los números de cuenta
        /// </summary>
        private readonly PersistentAccountNumber _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public AccountNumberTest()
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
        /// Prueba la consulta de un listado de números de cuenta con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<AccountNumber> list = _persistent.List("idaccountnumber = 1", "idaccountnumber", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de números de cuenta con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.List("idnumerocuenta = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un número de cuenta dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            AccountNumber number = new() { Id = 1 };

            //Act
            number = _persistent.Read(number);

            //Assert
            Assert.Equal("123456789", number.Number);
        }

        /// <summary>
        /// Prueba la consulta de un número de cuenta que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            AccountNumber number = new() { Id = 10 };

            //Act
            number = _persistent.Read(number);

            //Assert
            Assert.Equal(0, number.Id);
        }

        /// <summary>
        /// Prueba la consulta de un número de cuenta con error
        /// </summary>
        [Fact]
        public void ReadWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Read(null));
        }

        /// <summary>
        /// Prueba la inserción de un número de cuenta
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            AccountNumber number = new() { AccountType = new() { Id = 1 }, City = new() { Id = 1 }, Number = "9632587741" };

            //Act
            number = _persistent.Insert(number, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, number.Id);
        }

        /// <summary>
        /// Prueba la inserción de un número de cuenta con error
        /// </summary>
        [Fact]
        public void InsertWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Insert(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de un número de cuenta
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            AccountNumber number = new() { Id = 2, AccountType = new() { Id = 1 }, Number = "7531598264" };
            AccountNumber number2 = new() { Id = 2 };

            //Act
            _ = _persistent.Update(number, new() { Id = 1 });
            number2 = _persistent.Read(number2);

            //Assert
            Assert.NotEqual("987654321", number2.Number);
        }

        /// <summary>
        /// Prueba la actualización de un número de cuenta con error
        /// </summary>
        [Fact]
        public void UpdateWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Update(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un número de cuenta
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            AccountNumber number = new() { Id = 3 };
            AccountNumber number2 = new() { Id = 3 };

            //Act
            _ = _persistent.Delete(number, new() { Id = 1 });
            number2 = _persistent.Read(number2);

            //Assert
            Assert.Equal(0, number2.Id);
        }

        /// <summary>
        /// Prueba la eliminación de un número de cuenta con error
        /// </summary>
        [Fact]
        public void DeleteWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Delete(null, new() { Id = 1 }));
        }
        #endregion
    }
}