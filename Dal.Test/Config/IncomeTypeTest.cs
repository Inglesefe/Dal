using Dal.Config;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Config;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Dal.Test.Config
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de tipos de ingreso
    /// </summary>
    [Collection("Tests")]
    public class IncomeTypeTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de los tipos de ingreso
        /// </summary>
        private readonly PersistentIncomeType _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public IncomeTypeTest()
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
        /// Prueba la consulta de un listado de tipos de ingreso con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void IncomeTypeListTest()
        {
            ListResult<IncomeType> list = _persistent.List("idincometype = 1", "name", 1, 0, new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de tipos de ingreso con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void IncomeTypeListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idtipoingreso = 1", "name", 1, 0, new MySqlConnection(_configuration.GetConnectionString("golden") ?? "")));
        }

        /// <summary>
        /// Prueba la consulta de un tipo de ingreso dada su identificador
        /// </summary>
        [Fact]
        public void IncomeTypeReadTest()
        {
            IncomeType incomeType = new() { Id = 1 };
            incomeType = _persistent.Read(incomeType, new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));

            Assert.Equal("CI", incomeType.Code);
        }

        /// <summary>
        /// Prueba la consulta de un tipo de ingreso que no existe dado su identificador
        /// </summary>
        [Fact]
        public void IncomeTypeReadNotFoundTest()
        {
            IncomeType incomeType = new() { Id = 10 };
            incomeType = _persistent.Read(incomeType, new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));

            Assert.Equal(0, incomeType.Id);
        }

        /// <summary>
        /// Prueba la inserción de un tipo de ingreso
        /// </summary>
        [Fact]
        public void IncomeTypeInsertTest()
        {
            IncomeType incomeType = new() { Code = "CF", Name = "Cheques posfechados" };
            incomeType = _persistent.Insert(incomeType, new() { Id = 1 }, new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));

            Assert.NotEqual(0, incomeType.Id);
        }

        /// <summary>
        /// Prueba la actualización de un tipo de ingreso
        /// </summary>
        [Fact]
        public void IncomeTypeUpdateTest()
        {
            IncomeType incomeType = new() { Id = 2, Code = "CT", Name = "Otro ingreso" };
            _ = _persistent.Update(incomeType, new() { Id = 1 }, new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));

            IncomeType incomeType2 = new() { Id = 2 };
            incomeType2 = _persistent.Read(incomeType2, new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));

            Assert.NotEqual("Credito cartera", incomeType2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de un tipo de ingreso
        /// </summary>
        [Fact]
        public void IncomeTypeDeleteTest()
        {
            IncomeType incomeType = new() { Id = 3 };
            _ = _persistent.Delete(incomeType, new() { Id = 1 }, new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));

            IncomeType incomeType2 = new() { Id = 3 };
            incomeType2 = _persistent.Read(incomeType2, new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));

            Assert.Equal(0, incomeType2.Id);
        }
        #endregion
    }
}
