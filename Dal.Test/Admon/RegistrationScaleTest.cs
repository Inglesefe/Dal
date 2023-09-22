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
        /// Prueba la consulta de un listado de relaciones entre matrículas, escalas y ejecutivos de cuenta con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<RegistrationScale> list = _persistent.List("idregistration = 1", "", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de relaciones entre matrículas, escalas y ejecutivos de cuenta con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.List("idmatricula = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una relación entre matrícula, escala y ejecutivo de cuenta dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            RegistrationScale registration = new() { Id = 1 };

            //Act
            registration = _persistent.Read(registration);

            //Assert
            Assert.Equal(1, registration.Scale.Id);
        }

        /// <summary>
        /// Prueba la consulta de una relación entre matrícula, escala y ejecutivo de cuenta que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            RegistrationScale registration = new() { Id = 10 };

            //Act
            registration = _persistent.Read(registration);

            //Assert
            Assert.Equal(0, registration.Id);
        }

        /// <summary>
        /// Prueba la consulta de una relación entre matrícula, escala y ejecutivo de cuenta con error
        /// </summary>
        [Fact]
        public void ReadWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Read(null));
        }

        /// <summary>
        /// Prueba la inserción de una relación entre matrícula, escala y ejecutivo de cuenta
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            RegistrationScale registration = new()
            {
                Registration = new() { Id = 1 },
                Scale = new() { Id = 2 },
                AccountExecutive = new() { Id = 2 }
            };

            //Act
            registration = _persistent.Insert(registration, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, registration.Id);
        }

        /// <summary>
        /// Prueba la inserción de una relación entre matrícula, escala y ejecutivo de cuenta con error
        /// </summary>
        [Fact]
        public void InsertWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Insert(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de una relación entre matrícula, escala y ejecutivo de cuenta
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            RegistrationScale registration = new()
            {
                Id = 2,
                Scale = new() { Id = 2 },
                AccountExecutive = new() { Id = 4 }
            };
            RegistrationScale registration2 = new() { Id = 2 };

            //Act
            _ = _persistent.Update(registration, new() { Id = 1 });
            registration2 = _persistent.Read(registration2);

            //Assert
            Assert.NotEqual(2, registration2.AccountExecutive.Id);
        }

        /// <summary>
        /// Prueba la actualización de una relación entre matrícula, escala y ejecutivo de cuenta con error
        /// </summary>
        [Fact]
        public void UpdateWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Update(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de una relación entre matrícula, escala y ejecutivo de cuenta
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            RegistrationScale registration = new() { Id = 3 };
            RegistrationScale registration2 = new() { Id = 3 };

            //Act
            _ = _persistent.Delete(registration, new() { Id = 1 });
            registration2 = _persistent.Read(registration2);

            //Assert
            Assert.Equal(0, registration2.Id);
        }

        /// <summary>
        /// Prueba la eliminación de una relación entre matrícula, escala y ejecutivo de cuenta con error
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