using Dal.Config;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Config;
using Microsoft.Extensions.Configuration;

namespace Dal.Test.Config
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de parámetros
    /// </summary>
    [Collection("Tests")]
    public class ParameterTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de los parámetros
        /// </summary>
        private readonly PersistentParameter _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public ParameterTest()
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
        /// Prueba la consulta de un listado de parámetros con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<Parameter> list = _persistent.List("idparameter = 1", "name", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de parámetros con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.List("idparametro = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un parámetro dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            Parameter parameter = new() { Id = 1 };

            //Act
            parameter = _persistent.Read(parameter);

            //Assert
            Assert.Equal("Parametro 1", parameter.Name);
        }

        /// <summary>
        /// Prueba la consulta de un parámetro que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            Parameter parameter = new() { Id = 10 };

            //Act
            parameter = _persistent.Read(parameter);

            //Assert
            Assert.Equal(0, parameter.Id);
        }

        /// <summary>
        /// Prueba la consulta de un parámetro con error
        /// </summary>
        [Fact]
        public void ReadWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Read(null));
        }

        /// <summary>
        /// Prueba la inserción de un parámetro
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            Parameter parameter = new() { Name = "Parametro 4", Value = "Valor 4" };

            //Act
            parameter = _persistent.Insert(parameter, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, parameter.Id);
        }

        /// <summary>
        /// Prueba la inserción de un parámetro con nombre duplicado
        /// </summary>
        [Fact]
        public void InsertDuplicateTest()
        {
            //Arrange
            Parameter parameter = new() { Name = "Parametro 1", Value = "Valor 5" };

            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _persistent.Insert(parameter, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de un parámetro
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            Parameter parameter = new() { Id = 2, Name = "Parametro 6", Value = "Valor 6" };
            Parameter parameter2 = new() { Id = 2 };

            //Act
            _ = _persistent.Update(parameter, new() { Id = 1 });
            parameter2 = _persistent.Read(parameter2);

            //Assert
            Assert.NotEqual("Parametro 2", parameter2.Name);
        }

        /// <summary>
        /// Prueba la actualización de un parámetro con error
        /// </summary>
        [Fact]
        public void UpdateWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Update(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un parámetro
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            Parameter parameter = new() { Id = 3 };
            Parameter parameter2 = new() { Id = 3 };

            //Act
            _ = _persistent.Delete(parameter, new() { Id = 1 });
            parameter2 = _persistent.Read(parameter2);

            //Assert
            Assert.Equal(0, parameter2.Id);
        }

        /// <summary>
        /// Prueba la eliminación de un parámetro con error
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
