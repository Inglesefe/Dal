using Dal.Admon;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Admon;
using Microsoft.Extensions.Configuration;

namespace Dal.Test.Admon
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de matr�culas
    /// </summary>
    [Collection("Tests")]
    public class RegistrationTest
    {
        #region Attributes
        /// <summary>
        /// Configuraci�n de la aplicaci�n de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de las matr�culas
        /// </summary>
        private readonly PersistentRegistration _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuraci�n de la prueba
        /// </summary>
        public RegistrationTest()
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
        /// Prueba la consulta de un listado de matr�culas con filtros, ordenamientos y l�mite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            ListResult<Registration> list = _persistent.List("idregistration = 1", "", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de matr�culas con filtros, ordenamientos y l�mite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idmatricula = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una matr�cula dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            Registration registration = new() { Id = 1 };
            registration = _persistent.Read(registration);

            Assert.Equal("255657", registration.ContractNumber);
        }

        /// <summary>
        /// Prueba la consulta de una matr�cula que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            Registration registration = new() { Id = 10 };
            registration = _persistent.Read(registration);

            Assert.Equal(0, registration.Id);
        }

        /// <summary>
        /// Prueba la inserci�n de una matr�cula
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            Registration registration = new()
            {
                Office = new() { Id = 1 },
                Date = DateTime.Now,
                ContractNumber = "256567",
                Owner = new() { Id = 1 },
                Beneficiary1 = null,
                Beneficiary2 = null,
                Plan = new() { Id = 1 }
            };
            registration = _persistent.Insert(registration, new() { Id = 1 });

            Assert.NotEqual(0, registration.Id);
        }

        /// <summary>
        /// Prueba la actualizaci�n de una matr�cula
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            Registration registration = new()
            {
                Id = 2,
                Office = new() { Id = 1 },
                Date = DateTime.Now,
                ContractNumber = "256568",
                Owner = new() { Id = 1 },
                Beneficiary1 = null,
                Beneficiary2 = null,
                Plan = new() { Id = 1 }
            };
            _ = _persistent.Update(registration, new() { Id = 1 });

            Registration registration2 = new() { Id = 2 };
            registration2 = _persistent.Read(registration2);

            Assert.NotEqual("256566", registration2.ContractNumber);
        }

        /// <summary>
        /// Prueba la eliminaci�n de una matr�cula
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            Registration registration = new() { Id = 3 };
            _ = _persistent.Delete(registration, new() { Id = 1 });

            Registration registration2 = new() { Id = 3 };
            registration2 = _persistent.Read(registration2);

            Assert.Equal(0, registration2.Id);
        }
        #endregion
    }
}