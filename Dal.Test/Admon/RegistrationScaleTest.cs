using Dal.Admon;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Admon;
using Microsoft.Extensions.Configuration;

namespace Dal.Test.Admon
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de relaciones entre matrículas, escalas y ejecutivos de cuenta
    /// </summary>
    [Collection("Tests")]
    public class RegistrationScaleTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de las relaciones entre matrículas, escalas y ejecutivos de cuenta
        /// </summary>
        private readonly PersistentRegistrationScale _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public RegistrationScaleTest()
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
        /// Prueba la consulta de un listado de relaciones entre matrículas, escalas y ejecutivos de cuenta con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            ListResult<RegistrationScale> list = _persistent.List("idregistration = 1", "", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de relaciones entre matrículas, escalas y ejecutivos de cuenta con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idmatricula = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una relación entre matrícula, escala y ejecutivo de cuenta dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            RegistrationScale registration = new() { Id = 1 };
            registration = _persistent.Read(registration);

            Assert.Equal(1, registration.Scale.Id);
        }

        /// <summary>
        /// Prueba la consulta de una relación entre matrícula, escala y ejecutivo de cuenta que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            RegistrationScale registration = new() { Id = 10 };
            registration = _persistent.Read(registration);

            Assert.Equal(0, registration.Id);
        }

        /// <summary>
        /// Prueba la inserción de una relación entre matrícula, escala y ejecutivo de cuenta
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            RegistrationScale registration = new()
            {
                Registration = new() { Id = 1 },
                Scale = new() { Id = 2 },
                AccountExecutive = new() { Id = 2 }
            };
            registration = _persistent.Insert(registration, new() { Id = 1 });

            Assert.NotEqual(0, registration.Id);
        }

        /// <summary>
        /// Prueba la actualización de una relación entre matrícula, escala y ejecutivo de cuenta
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            RegistrationScale registration = new()
            {
                Id = 2,
                Scale = new() { Id = 2 },
                AccountExecutive = new() { Id = 4 }
            };
            _ = _persistent.Update(registration, new() { Id = 1 });

            RegistrationScale registration2 = new() { Id = 2 };
            registration2 = _persistent.Read(registration2);

            Assert.NotEqual(2, registration2.AccountExecutive.Id);
        }

        /// <summary>
        /// Prueba la eliminación de una relación entre matrícula, escala y ejecutivo de cuenta
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            RegistrationScale registration = new() { Id = 3 };
            _ = _persistent.Delete(registration, new() { Id = 1 });

            RegistrationScale registration2 = new() { Id = 3 };
            registration2 = _persistent.Read(registration2);

            Assert.Equal(0, registration2.Id);
        }
        #endregion
    }
}