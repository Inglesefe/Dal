using Dal.Dto;
using Dal.Exceptions;
using Dal.Log;
using Entities.Log;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

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
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .AddEnvironmentVariables()
                .Build();
            _persistent = new();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de registros con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void LogComponentListTest()
        {
            ListResult<LogComponent> list = _persistent.List("idlog = 1", "idlog", 1, 0, new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de registros con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void LogComponentListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idregistro = 1", "name", 1, 0, new MySqlConnection(_configuration.GetConnectionString("golden") ?? "")));
        }

        /// <summary>
        /// Prueba la consulta de un registro dada su identificador
        /// </summary>
        [Fact]
        public void LogComponentReadTest()
        {
            LogComponent log = new() { Id = 1 };
            log = _persistent.Read(log, new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));

            Assert.Equal("Log.Test", log.Component);
        }

        /// <summary>
        /// Prueba la consulta de un registro que no existe dado su identificador
        /// </summary>
        [Fact]
        public void LogComponentReadNotFoundTest()
        {
            LogComponent log = new() { Id = 10 };
            log = _persistent.Read(log, new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));

            Assert.Equal(0, log.Id);
        }

        /// <summary>
        /// Prueba la inserción de un registro
        /// </summary>
        [Fact]
        public void LogComponentInsertTest()
        {
            LogComponent log = new() { Type = "I", Component = "Log.Test", Description = "Prueba de insercion de log con usuario", User = 1 };
            log = _persistent.Insert(log, new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));

            Assert.NotEqual(0, log.Id);
        }

        /// <summary>
        /// Prueba la actualización de un registro
        /// </summary>
        [Fact]
        public void LogComponentUpdateTest()
        {
            LogComponent log = new() { Id = 1 };
            log = _persistent.Update(log, new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));

            Assert.Equal(1, log.Id);
        }

        /// <summary>
        /// Prueba la eliminación de un registro
        /// </summary>
        [Fact]
        public void LogComponentDeleteTest()
        {
            LogComponent log = new() { Id = 1 };
            log = _persistent.Delete(log, new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));

            Assert.Equal(1, log.Id);
        }
        #endregion
    }
}