using Dal.Config;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Config;
using Microsoft.Extensions.Configuration;

namespace Dal.Test.Config
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de tipos de identificación
    /// </summary>
    [Collection("Tests")]
    public class IdentificationTypeTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de los tipos de identificación
        /// </summary>
        private readonly PersistentIdentificationType _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public IdentificationTypeTest()
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
        /// Prueba la consulta de un listado de tipos de identificación con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            ListResult<IdentificationType> list = _persistent.List("ididentificationtype = 1", "name", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de tipos de identificación con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idtipoidentificacion = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un tipo de identificación dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            IdentificationType identificationType = new() { Id = 1 };
            identificationType = _persistent.Read(identificationType);

            Assert.Equal("Cedula ciudadania", identificationType.Name);
        }

        /// <summary>
        /// Prueba la consulta de un tipo de identificación que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            IdentificationType identificationType = new() { Id = 10 };
            identificationType = _persistent.Read(identificationType);

            Assert.Equal(0, identificationType.Id);
        }

        /// <summary>
        /// Prueba la inserción de un tipo de identificación
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            IdentificationType identificationType = new() { Name = "Prueba 1" };
            identificationType = _persistent.Insert(identificationType, new() { Id = 1 });

            Assert.NotEqual(0, identificationType.Id);
        }

        /// <summary>
        /// Prueba la actualización de un tipo de identificación
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            IdentificationType identificationType = new() { Id = 2, Name = "Tarjeta de identidad" };
            _ = _persistent.Update(identificationType, new() { Id = 1 });

            IdentificationType identificationType2 = new() { Id = 2 };
            identificationType2 = _persistent.Read(identificationType2);

            Assert.NotEqual("Cedula extranjeria", identificationType2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de un tipo de identificación
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            IdentificationType identificationType = new() { Id = 3 };
            _ = _persistent.Delete(identificationType, new() { Id = 1 });

            IdentificationType identificationType2 = new() { Id = 3 };
            identificationType2 = _persistent.Read(identificationType2);

            Assert.Equal(0, identificationType2.Id);
        }
        #endregion
    }
}
