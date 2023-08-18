using Dal.Config;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Config;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

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
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .AddEnvironmentVariables()
                .Build();
            _persistent = new(new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de parámetros con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ParameterListTest()
        {
            ListResult<Parameter> list = _persistent.List("idparameter = 1", "name", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de parámetros con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ParameterListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idparametro = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un parámetro dada su identificador
        /// </summary>
        [Fact]
        public void ParameterReadTest()
        {
            Parameter parameter = new() { Id = 1 };
            parameter = _persistent.Read(parameter);

            Assert.Equal("Parametro 1", parameter.Name);
        }

        /// <summary>
        /// Prueba la consulta de un parámetro que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ParameterReadNotFoundTest()
        {
            Parameter parameter = new() { Id = 10 };
            parameter = _persistent.Read(parameter);

            Assert.Null(parameter);
        }

        /// <summary>
        /// Prueba la inserción de un parámetro
        /// </summary>
        [Fact]
        public void ParameterInsertTest()
        {
            Parameter parameter = new() { Name = "Parametro 4", Value = "Valor 4" };
            parameter = _persistent.Insert(parameter, new() { Id = 1 });

            Assert.NotEqual(0, parameter.Id);
        }

        /// <summary>
        /// Prueba la inserción de un parámetro con nombre duplicado
        /// </summary>
        [Fact]
        public void ParameterInsertDuplicateTest()
        {
            Parameter parameter = new() { Name = "Parametro 1", Value = "Valor 5" };

            _ = Assert.Throws<PersistentException>(() => _persistent.Insert(parameter, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de un parámetro
        /// </summary>
        [Fact]
        public void ParameterUpdateTest()
        {
            Parameter parameter = new() { Id = 2, Name = "Parametro 6", Value = "Valor 6" };
            _ = _persistent.Update(parameter, new() { Id = 1 });

            Parameter parameter2 = new() { Id = 2 };
            parameter2 = _persistent.Read(parameter2);

            Assert.NotEqual("Parametro 2", parameter2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de un parámetro
        /// </summary>
        [Fact]
        public void ParameterDeleteTest()
        {
            Parameter parameter = new() { Id = 3 };
            _ = _persistent.Delete(parameter, new() { Id = 1 });

            Parameter parameter2 = new() { Id = 3 };
            parameter2 = _persistent.Read(parameter2);

            Assert.Null(parameter2);
        }
        #endregion
    }
}
