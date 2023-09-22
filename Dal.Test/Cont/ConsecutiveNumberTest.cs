using Dal.Cont;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Cont;
using Microsoft.Extensions.Configuration;

namespace Dal.Test.Cont
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de números de consecutivo
    /// </summary>
    [Collection("Tests")]
    public class ConsecutiveNumberTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de los números de consecutivo
        /// </summary>
        private readonly PersistentConsecutiveNumber _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public ConsecutiveNumberTest()
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
        /// Prueba la consulta de un listado de números de consecutivo con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<ConsecutiveNumber> list = _persistent.List("idconsecutivenumber = 1", "idconsecutivenumber", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de números de consecutivo con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.List("idnumeroconsecutivo = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un número de consecutivo dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            ConsecutiveNumber number = new() { Id = 1 };

            //Act
            number = _persistent.Read(number);

            //Assert
            Assert.Equal("9999999999", number.Number);
        }

        /// <summary>
        /// Prueba la consulta de un número de consecutivo que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            ConsecutiveNumber number = new() { Id = 10 };

            //Act
            number = _persistent.Read(number);

            //Assert
            Assert.Equal(0, number.Id);
        }

        /// <summary>
        /// Prueba la consulta de un número de consecutivo con error
        /// </summary>
        [Fact]
        public void ReadWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Read(null));
        }

        /// <summary>
        /// Prueba la inserción de un número de consecutivo
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            ConsecutiveNumber number = new() { ConsecutiveType = new() { Id = 1 }, City = new() { Id = 1 }, Number = "66666666666" };

            //Act
            number = _persistent.Insert(number, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, number.Id);
        }

        /// <summary>
        /// Prueba la inserción de un número de consecutivo con error
        /// </summary>
        [Fact]
        public void InsertWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Insert(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de un número de consecutivo
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            ConsecutiveNumber number = new() { Id = 2, ConsecutiveType = new() { Id = 1 }, Number = "55555555555" };
            ConsecutiveNumber number2 = new() { Id = 2 };

            //Act
            _ = _persistent.Update(number, new() { Id = 1 });
            number2 = _persistent.Read(number2);

            //Assert
            Assert.NotEqual("8888888888", number2.Number);
        }

        /// <summary>
        /// Prueba la actualización de un número de consecutivo con error
        /// </summary>
        [Fact]
        public void UpdateWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Update(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un número de consecutivo
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            ConsecutiveNumber number = new() { Id = 3 };
            ConsecutiveNumber number2 = new() { Id = 3 };

            //Act
            _ = _persistent.Delete(number, new() { Id = 1 });
            number2 = _persistent.Read(number2);

            //Assert
            Assert.Equal(0, number2.Id);
        }

        /// <summary>
        /// Prueba la eliminación de un número de consecutivo con error
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