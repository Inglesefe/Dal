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
        /// Prueba la consulta de un listado de tipos de identificación con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<IdentificationType> list = _persistent.List("ididentificationtype = 1", "name", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de tipos de identificación con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.List("idtipoidentificacion = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un tipo de identificación dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            IdentificationType identificationType = new() { Id = 1 };

            //Act
            identificationType = _persistent.Read(identificationType);

            //Assert
            Assert.Equal("Cedula ciudadania", identificationType.Name);
        }

        /// <summary>
        /// Prueba la consulta de un tipo de identificación que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            IdentificationType identificationType = new() { Id = 10 };

            //Act
            identificationType = _persistent.Read(identificationType);

            //Assert
            Assert.Equal(0, identificationType.Id);
        }

        /// <summary>
        /// Prueba la consulta de un tipo de identificación con error
        /// </summary>
        [Fact]
        public void ReadWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Read(null));
        }

        /// <summary>
        /// Prueba la inserción de un tipo de identificación
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            IdentificationType identificationType = new() { Name = "Prueba 1" };

            //Act
            identificationType = _persistent.Insert(identificationType, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, identificationType.Id);
        }

        /// <summary>
        /// Prueba la inserción de un tipo de identificación con error
        /// </summary>
        [Fact]
        public void InsertWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Insert(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de un tipo de identificación
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            IdentificationType identificationType = new() { Id = 2, Name = "Tarjeta de identidad" };
            IdentificationType identificationType2 = new() { Id = 2 };

            //Act
            _ = _persistent.Update(identificationType, new() { Id = 1 });
            identificationType2 = _persistent.Read(identificationType2);

            //Assert
            Assert.NotEqual("Cedula extranjeria", identificationType2.Name);
        }

        /// <summary>
        /// Prueba la actualización de un tipo de identificación con error
        /// </summary>
        [Fact]
        public void UpdateWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Update(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un tipo de identificación
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            IdentificationType identificationType = new() { Id = 3 };
            IdentificationType identificationType2 = new() { Id = 3 };

            //Act
            _ = _persistent.Delete(identificationType, new() { Id = 1 });
            identificationType2 = _persistent.Read(identificationType2);

            //Assert
            Assert.Equal(0, identificationType2.Id);
        }

        /// <summary>
        /// Prueba la eliminación de un tipo de identificación con error
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
