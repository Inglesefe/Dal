using Dal.Admon;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Admon;
using Microsoft.Extensions.Configuration;

namespace Dal.Test.Admon
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de ejecutivos de cuenta
    /// </summary>
    [Collection("Tests")]
    public class AccountExecutiveTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de los ejecutivos de cuenta
        /// </summary>
        private readonly PersistentAccountExecutive _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public AccountExecutiveTest()
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
        /// Prueba la consulta de un listado de ejecutivos de cuenta con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            ListResult<AccountExecutive> list = _persistent.List("idaccountexecutive = 1", "", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de ejecutivos de cuenta con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idejecutivocuenta = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un ejecutivo de cuenta dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            AccountExecutive executive = new() { Id = 1 };
            executive = _persistent.Read(executive);

            Assert.Equal("Leandro Baena Torres", executive.Name);
        }

        /// <summary>
        /// Prueba la consulta de un ejecutivo de cuenta que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            AccountExecutive executive = new() { Id = 10 };
            executive = _persistent.Read(executive);

            Assert.Equal(0, executive.Id);
        }

        /// <summary>
        /// Prueba la inserción de un ejecutivo de cuenta
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            AccountExecutive executive = new() { Name = "Prueba 1", IdentificationType = new() { Id = 1 }, Identification = "963258741" };
            executive = _persistent.Insert(executive, new() { Id = 1 });

            Assert.NotEqual(0, executive.Id);
        }

        /// <summary>
        /// Prueba la inserción de un ejecutivo de cuenta con identificación duplicada
        /// </summary>
        [Fact]
        public void InsertDuplicateTest()
        {
            AccountExecutive executive = new() { Name = "Error", IdentificationType = new() { Id = 1 }, Identification = "123456789" };

            _ = Assert.Throws<PersistentException>(() => _persistent.Insert(executive, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de un ejecutivo de cuenta
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            AccountExecutive executive = new() { Id = 2, Name = "Prueba actualizar", IdentificationType = new() { Id = 1 }, Identification = "1597536482" };
            _ = _persistent.Update(executive, new() { Id = 1 });

            AccountExecutive executive2 = new() { Id = 2 };
            executive2 = _persistent.Read(executive2);

            Assert.NotEqual("David Santiago Baena Barreto", executive2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de un ejecutivo de cuenta
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            AccountExecutive executive = new() { Id = 3 };
            _ = _persistent.Delete(executive, new() { Id = 1 });

            AccountExecutive executive2 = new() { Id = 3 };
            executive2 = _persistent.Read(executive2);

            Assert.Equal(0, executive2.Id);
        }
        #endregion
    }
}