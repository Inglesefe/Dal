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
            ListResult<AccountType> list = _persistent.List("idaccounttype = 1", "name", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de tipos de cuenta con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idtipocuenta = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un tipo de cuenta dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            AccountType type = new() { Id = 1 };
            type = _persistent.Read(type);

            Assert.Equal("Caja", type.Name);
        }

        /// <summary>
        /// Prueba la consulta de un tipo de cuenta que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            AccountType type = new() { Id = 10 };
            type = _persistent.Read(type);

            Assert.Equal(0, type.Id);
        }

        /// <summary>
        /// Prueba la inserción de un tipo de cuenta
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            AccountType type = new() { Name = "Prueba 1" };
            type = _persistent.Insert(type, new() { Id = 1 });

            Assert.NotEqual(0, type.Id);
        }

        /// <summary>
        /// Prueba la actualización de un tipo de cuenta
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            AccountType type = new() { Id = 2, Name = "Prueba actualizar" };
            _ = _persistent.Update(type, new() { Id = 1 });

            AccountType type2 = new() { Id = 2 };
            type2 = _persistent.Read(type2);

            Assert.NotEqual("Bancos", type2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de un tipo de cuenta
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            AccountType type = new() { Id = 3 };
            _ = _persistent.Delete(type, new() { Id = 1 });

            AccountType type2 = new() { Id = 3 };
            type2 = _persistent.Read(type2);

            Assert.Equal(0, type2.Id);
        }
        #endregion
    }
}