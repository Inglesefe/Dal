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
        /// Prueba la consulta de un listado de ciudades con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<City> list = _persistent.List("idcountry = 1", "idcountry", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de ciudades con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.List("idpais = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una ciudad dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            City city = new() { Id = 1 };

            //Act
            city = _persistent.Read(city);

            //Assert
            Assert.Equal("BOG", city.Code);
        }

        /// <summary>
        /// Prueba la consulta de una ciudad que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            City city = new() { Id = 10 };

            //Act
            city = _persistent.Read(city);

            //Assert
            Assert.Equal(0, city.Id);
        }

        /// <summary>
        /// Prueba la consulta de una ciudad con error
        /// </summary>
        [Fact]
        public void ReadWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Read(null));
        }

        /// <summary>
        /// Prueba la inserción de una ciudad
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            City city = new() { Country = new() { Id = 1 }, Code = "BUC", Name = "Bucaramanga" };

            //Act
            city = _persistent.Insert(city, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, city.Id);
        }

        /// <summary>
        /// Prueba la inserción de una ciudad con código duplicado
        /// </summary>
        [Fact]
        public void InsertDuplicateTest()
        {
            //Arrange
            City city = new() { Country = new() { Id = 1 }, Code = "BOG", Name = "Prueba 1" };

            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _persistent.Insert(city, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de una ciudad
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            City city = new() { Id = 2, Country = new() { Id = 1 }, Code = "BAQ", Name = "Barranquilla" };
            City country2 = new() { Id = 2 };

            //Act
            _ = _persistent.Update(city, new() { Id = 1 });
            country2 = _persistent.Read(country2);

            //Assert
            Assert.NotEqual("MED", country2.Code);
        }

        /// <summary>
        /// Prueba la actualización de una ciudad con error
        /// </summary>
        [Fact]
        public void UpdateWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Update(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de una ciudad
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            City city = new() { Id = 3 };
            City city2 = new() { Id = 3 };

            //Act
            _ = _persistent.Delete(city, new() { Id = 1 });
            city2 = _persistent.Read(city2);

            //Assert
            Assert.Equal(0, city2.Id);
        }

        /// <summary>
        /// Prueba la eliminación de una ciudad con error
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
