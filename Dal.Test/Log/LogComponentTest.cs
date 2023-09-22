using Dal.Dto;
using Dal.Exceptions;
using Dal.Log;
using Entities.Log;
using Microsoft.Extensions.Configuration;

namespace Dal.Test.Log
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de registro de componentes
    /// </summary>
    [Collection("Tests")]
    public class LogComponentTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de persistencia en la base de datos
        /// </summary>
        private readonly PersistentLogComponent _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public LogComponentTest()
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
        /// Prueba la consulta de un listado de registros con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<LogComponent> list = _persistent.List("idlog = 1", "idlog", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de registros con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.List("idregistro = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un registro dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            LogComponent log = new() { Id = 1 };

            //Act
            log = _persistent.Read(log);

            //Assert
            Assert.Equal("Log", log.Controller);
        }

        /// <summary>
        /// Prueba la consulta de un registro que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            LogComponent log = new() { Id = 10 };

            //Act
            log = _persistent.Read(log);

            //Assert
            Assert.Equal(0, log.Id);
        }

        /// <summary>
        /// Prueba la consulta de un registro con error
        /// </summary>
        [Fact]
        public void ReadWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Read(null));
        }

        /// <summary>
        /// Prueba la inserción de un registro
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            LogComponent log = new() { Type = "I", Controller = "Log3", Method = "Test3", Input = "Entrada 3", Output = "Salida 3", User = 1 };

            //Act
            log = _persistent.Insert(log);

            //Assert
            Assert.NotEqual(0, log.Id);
        }

        /// <summary>
        /// Prueba la inserción de un registro con error
        /// </summary>
        [Fact]
        public void InsertWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Insert(null));
        }

        /// <summary>
        /// Prueba la actualización de un registro
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            LogComponent log = new() { Id = 1 };

            //Act
            log = _persistent.Update(log);

            //Assert
            Assert.Equal(1, log.Id);
        }

        /// <summary>
        /// Prueba la eliminación de un registro
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            LogComponent log = new() { Id = 1 };

            //Act
            log = _persistent.Delete(log);

            //Assert
            Assert.Equal(1, log.Id);
        }
        #endregion
    }
}