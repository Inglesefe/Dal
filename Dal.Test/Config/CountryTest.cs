using Dal.Config;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Config;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Dal.Test.Config
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de paises
    /// </summary>
    [Collection("Tests")]
    public class CountryTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de los paises
        /// </summary>
        private readonly PersistentCountry _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public CountryTest()
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
        /// Prueba la consulta de un listado de paises con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void CountryListTest()
        {
            ListResult<Country> list = _persistent.List("idcountry = 1", "name", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de paises con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void CountryListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idpais = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un país dada su identificador
        /// </summary>
        [Fact]
        public void CountryReadTest()
        {
            Country country = new() { Id = 1 };
            country = _persistent.Read(country);

            Assert.Equal("CO", country.Code);
        }

        /// <summary>
        /// Prueba la consulta de un país que no existe dado su identificador
        /// </summary>
        [Fact]
        public void CountryReadNotFoundTest()
        {
            Country country = new() { Id = 10 };
            country = _persistent.Read(country);

            Assert.Null(country);
        }

        /// <summary>
        /// Prueba la inserción de un país
        /// </summary>
        [Fact]
        public void CountryInsertTest()
        {
            Country country = new() { Code = "PR", Name = "Puerto Rico" };
            country = _persistent.Insert(country, new() { Id = 1 });

            Assert.NotEqual(0, country.Id);
        }

        /// <summary>
        /// Prueba la inserción de un país con código duplicado
        /// </summary>
        [Fact]
        public void CountryInsertDuplicateTest()
        {
            Country country = new() { Code = "CO", Name = "Colombia" };

            _ = Assert.Throws<PersistentException>(() => _persistent.Insert(country, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de un país
        /// </summary>
        [Fact]
        public void CountryUpdateTest()
        {
            Country country = new() { Id = 2, Code = "PE", Name = "Perú" };
            _ = _persistent.Update(country, new() { Id = 1 });

            Country country2 = new() { Id = 2 };
            country2 = _persistent.Read(country2);

            Assert.NotEqual("US", country2.Code);
        }

        /// <summary>
        /// Prueba la eliminación de un país
        /// </summary>
        [Fact]
        public void CountryDeleteTest()
        {
            Country country = new() { Id = 3 };
            _ = _persistent.Delete(country, new() { Id = 1 });

            Country country2 = new() { Id = 3 };
            country2 = _persistent.Read(country2);

            Assert.Null(country2);
        }
        #endregion
    }
}
