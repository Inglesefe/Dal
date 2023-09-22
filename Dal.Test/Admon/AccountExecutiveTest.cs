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
        /// Prueba la consulta de un listado de ejecutivos de cuenta con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<AccountExecutive> list = _persistent.List("idaccountexecutive = 1", "", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de ejecutivos de cuenta con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.List("idejecutivocuenta = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un ejecutivo de cuenta dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            AccountExecutive executive = new() { Id = 1 };

            //Act
            executive = _persistent.Read(executive);

            //Assert
            Assert.Equal("Leandro Baena Torres", executive.Name);
        }

        /// <summary>
        /// Prueba la consulta de un ejecutivo de cuenta que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            AccountExecutive executive = new() { Id = 10 };

            //Act
            executive = _persistent.Read(executive);

            //Assert
            Assert.Equal(0, executive.Id);
        }

        /// <summary>
        /// Prueba la consulta de un ejecutivo de cuenta con error
        /// </summary>
        [Fact]
        public void ReadWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Read(null));

        }

        /// <summary>
        /// Prueba la inserción de un ejecutivo de cuenta
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            AccountExecutive executive = new() { Name = "Prueba 1", IdentificationType = new() { Id = 1 }, Identification = "963258741" };

            //Act
            executive = _persistent.Insert(executive, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, executive.Id);
        }

        /// <summary>
        /// Prueba la inserción de un ejecutivo de cuenta con identificación duplicada
        /// </summary>
        [Fact]
        public void InsertDuplicateTest()
        {
            //Arrange
            AccountExecutive executive = new() { Name = "Error", IdentificationType = new() { Id = 1 }, Identification = "123456789" };

            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _persistent.Insert(executive, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de un ejecutivo de cuenta
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            AccountExecutive executive = new() { Id = 2, Name = "Prueba actualizar", IdentificationType = new() { Id = 1 }, Identification = "1597536482" };
            AccountExecutive executive2 = new() { Id = 2 };

            //Act
            _ = _persistent.Update(executive, new() { Id = 1 });
            executive2 = _persistent.Read(executive2);

            //Assert
            Assert.NotEqual("David Santiago Baena Barreto", executive2.Name);
        }

        /// <summary>
        /// Prueba la actualización de un ejecutivo de cuenta con error
        /// </summary>
        [Fact]
        public void UpdateWithErrorTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _persistent.Update(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un ejecutivo de cuenta
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            AccountExecutive executive = new() { Id = 3 };
            AccountExecutive executive2 = new() { Id = 3 };

            //Act
            _ = _persistent.Delete(executive, new() { Id = 1 });
            executive2 = _persistent.Read(executive2);

            //Assert
            Assert.Equal(0, executive2.Id);
        }

        /// <summary>
        /// Prueba la eliminación de un ejecutivo de cuenta con error
        /// </summary>
        [Fact]
        public void DeleteWithErrorTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _persistent.Delete(null, new() { Id = 1 }));
        }
        #endregion
    }
}