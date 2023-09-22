using Dal.Cont;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Cont;
using Microsoft.Extensions.Configuration;

namespace Dal.Test.Cont
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de tipos de cuenta
    /// </summary>
    [Collection("Tests")]
    public class AccountTypeTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de los tipos de cuenta
        /// </summary>
        private readonly PersistentAccountType _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public AccountTypeTest()
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
        /// Prueba la consulta de un listado de tipos de cuenta con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<AccountType> list = _persistent.List("idaccounttype = 1", "name", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de tipos de cuenta con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.List("idtipocuenta = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un tipo de cuenta dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            AccountType type = new() { Id = 1 };

            //Act
            type = _persistent.Read(type);

            //Assert
            Assert.Equal("Caja", type.Name);
        }

        /// <summary>
        /// Prueba la consulta de un tipo de cuenta que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            AccountType type = new() { Id = 10 };

            //Act
            type = _persistent.Read(type);

            //Assert
            Assert.Equal(0, type.Id);
        }

        /// <summary>
        /// Prueba la consulta de un tipo de cuenta con error
        /// </summary>
        [Fact]
        public void ReadWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Read(null));
        }

        /// <summary>
        /// Prueba la inserción de un tipo de cuenta
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            AccountType type = new() { Name = "Prueba 1" };

            //Act
            type = _persistent.Insert(type, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, type.Id);
        }

        /// <summary>
        /// Prueba la inserción de un tipo de cuenta con error
        /// </summary>
        [Fact]
        public void InsertWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Insert(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de un tipo de cuenta
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            AccountType type = new() { Id = 2, Name = "Prueba actualizar" };
            AccountType type2 = new() { Id = 2 };

            //Act
            _ = _persistent.Update(type, new() { Id = 1 });
            type2 = _persistent.Read(type2);

            //Assert
            Assert.NotEqual("Bancos", type2.Name);
        }

        /// <summary>
        /// Prueba la actualización de un tipo de cuenta con error
        /// </summary>
        [Fact]
        public void UpdateWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Update(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un tipo de cuenta
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            AccountType type = new() { Id = 3 };
            AccountType type2 = new() { Id = 3 };

            //Act
            _ = _persistent.Delete(type, new() { Id = 1 });
            type2 = _persistent.Read(type2);

            //Assert
            Assert.Equal(0, type2.Id);
        }

        /// <summary>
        /// Prueba la eliminación de un tipo de cuenta con error
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