using Dal.Config;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Config;
using Microsoft.Extensions.Configuration;

namespace Dal.Test.Config
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de ciudades
    /// </summary>
    [Collection("Tests")]
    public class CityTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de las ciudades
        /// </summary>
        private readonly PersistentCity _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public CityTest()
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
        /// Prueba la consulta de un listado de ciudades con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            ListResult<City> list = _persistent.List("idcountry = 1", "idcountry", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de ciudades con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idpais = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una ciudad dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            City city = new() { Id = 1 };
            city = _persistent.Read(city);

            Assert.Equal("BOG", city.Code);
        }

        /// <summary>
        /// Prueba la consulta de una ciudad que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            City city = new() { Id = 10 };
            city = _persistent.Read(city);

            Assert.Equal(0, city.Id);
        }

        /// <summary>
        /// Prueba la inserción de una ciudad
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            City city = new() { Country = new() { Id = 1 }, Code = "BUC", Name = "Bucaramanga" };
            city = _persistent.Insert(city, new() { Id = 1 });

            Assert.NotEqual(0, city.Id);
        }

        /// <summary>
        /// Prueba la inserción de una ciudad con código duplicado
        /// </summary>
        [Fact]
        public void InsertDuplicateTest()
        {
            City city = new() { Country = new() { Id = 1 }, Code = "BOG", Name = "Prueba 1" };

            _ = Assert.Throws<PersistentException>(() => _persistent.Insert(city, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de una ciudad
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            City city = new() { Id = 2, Country = new() { Id = 1 }, Code = "BAQ", Name = "Barranquilla" };
            _ = _persistent.Update(city, new() { Id = 1 });

            City country2 = new() { Id = 2 };
            country2 = _persistent.Read(country2);

            Assert.NotEqual("MED", country2.Code);
        }

        /// <summary>
        /// Prueba la eliminación de una ciudad
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            City city = new() { Id = 3 };
            _ = _persistent.Delete(city, new() { Id = 1 });

            City city2 = new() { Id = 3 };
            city2 = _persistent.Read(city2);

            Assert.Equal(0, city2.Id);
        }
        #endregion
    }
}
