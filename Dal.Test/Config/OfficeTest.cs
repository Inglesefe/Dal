using Dal.Config;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Config;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Dal.Test.Config
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de oficinas
    /// </summary>
    [Collection("Tests")]
    public class OfficeTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de las oficinas
        /// </summary>
        private readonly PersistentOffice _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public OfficeTest()
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
        /// Prueba la consulta de un listado de oficinas con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void OfficeListTest()
        {
            ListResult<Office> list = _persistent.List("o.idoffice = 1", "o.name", 1, 0, new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de oficinas con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void OfficeListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idoficina = 1", "name", 1, 0, new MySqlConnection(_configuration.GetConnectionString("golden") ?? "")));
        }

        /// <summary>
        /// Prueba la consulta de una oficina dada su identificador
        /// </summary>
        [Fact]
        public void OfficeReadTest()
        {
            Office office = new() { Id = 1 };
            office = _persistent.Read(office, new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));

            Assert.Equal("Castellana", office.Name);
        }

        /// <summary>
        /// Prueba la consulta de una oficina que no existe dado su identificador
        /// </summary>
        [Fact]
        public void OfficeReadNotFoundTest()
        {
            Office office = new() { Id = 10 };
            office = _persistent.Read(office, new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));

            Assert.Equal(0, office.Id);
        }

        /// <summary>
        /// Prueba la inserción de una oficina
        /// </summary>
        [Fact]
        public void OfficeInsertTest()
        {
            Office office = new() { City = new() { Id = 1 }, Name = "Madelena", Address = "Calle 59 sur" };
            office = _persistent.Insert(office, new() { Id = 1 }, new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));

            Assert.NotEqual(0, office.Id);
        }

        /// <summary>
        /// Prueba la actualización de una oficina
        /// </summary>
        [Fact]
        public void OfficeUpdateTest()
        {
            Office city = new() { Id = 2, City = new() { Id = 1 }, Name = "Santa Librada", Address = "Calle 78 sur" };
            _ = _persistent.Update(city, new() { Id = 1 }, new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));

            Office city2 = new() { Id = 2 };
            city2 = _persistent.Read(city2, new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));

            Assert.NotEqual("Kennedy", city2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de una oficina
        /// </summary>
        [Fact]
        public void OfficeDeleteTest()
        {
            Office office = new() { Id = 3 };
            _ = _persistent.Delete(office, new() { Id = 1 }, new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));

            Office office2 = new() { Id = 3 };
            office2 = _persistent.Read(office2, new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));

            Assert.Equal(0, office2.Id);
        }
        #endregion
    }
}
