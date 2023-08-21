using Dal.Dto;
using Dal.Exceptions;
using Dal.Log;
using Entities.Log;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Dal.Test.Log
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de registros de base de datos
    /// </summary>
    [Collection("Tests")]
    public class LogDbTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de persistencia en la base de datos
        /// </summary>
        private readonly PersistentLogDb _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public LogDbTest()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .AddEnvironmentVariables()
                .Build();
            _persistent = new(new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de registros con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void LogDbListTest()
        {
            ListResult<LogDb> list = _persistent.List("idlog = 1", "idlog", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de registros con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void LogDbListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idregistro = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un registro dada su identificador
        /// </summary>
        [Fact]
        public void LogDbReadTest()
        {
            LogDb log = new() { Id = 1 };
            log = _persistent.Read(log);

            Assert.Equal("Tabla1", log.Table);
        }

        /// <summary>
        /// Prueba la consulta de un registro que no existe dado su identificador
        /// </summary>
        [Fact]
        public void LogDbReadNotFoundTest()
        {
            LogDb log = new() { Id = 1000 };
            log = _persistent.Read(log);

            Assert.Equal(0, log.Id);
        }

        /// <summary>
        /// Prueba la inserción de un registro
        /// </summary>
        [Fact]
        public void LogDbInsertTest()
        {
            LogDb log = new() { Action = "I", IdTable = 1, Table = "Tabla1", Sql = "INSERT INTO Tabla1 (campo1) VALUES ('prueba insercion')", User = 1 };
            log = _persistent.Insert(log);

            Assert.NotEqual(0, log.Id);
        }
        #endregion
    }
}