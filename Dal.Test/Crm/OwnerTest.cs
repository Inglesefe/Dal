using Dal.Crm;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Crm;
using Microsoft.Extensions.Configuration;

namespace Dal.Test.Crm
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de titulares
    /// </summary>
    [Collection("Tests")]
    public class OwnerTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de los titulares
        /// </summary>
        private readonly PersistentOwner _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public OwnerTest()
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
        /// Prueba la consulta de un listado de titulares con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<Owner> list = _persistent.List("idowner = 1", "idowner", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de titulares con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.List("idtitular = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un titular dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            Owner owner = new() { Id = 1 };

            //Act
            owner = _persistent.Read(owner);

            //Assert
            Assert.Equal("Leandro Baena Torres", owner.Name);
        }

        /// <summary>
        /// Prueba la consulta de un titular que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            Owner owner = new() { Id = 10 };

            //Act
            owner = _persistent.Read(owner);

            //Assert
            Assert.Equal(0, owner.Id);
        }

        /// <summary>
        /// Prueba la consulta de un titular con error
        /// </summary>
        [Fact]
        public void ReadWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Read(null));
        }

        /// <summary>
        /// Prueba la inserción de un titular
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            Owner owner = new()
            {
                Name = "Prueba 1",
                IdentificationType = new() { Id = 1 },
                Identification = "963258741",
                AddressHome = "CL 19 # 20 - 21",
                AddressOffice = "CL 22 # 23 - 24",
                PhoneHome = "3571594682",
                PhoneOffice = "2864951753",
                Email = "prueba@prueba.com"
            };

            //Act
            owner = _persistent.Insert(owner, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, owner.Id);
        }

        /// <summary>
        /// Prueba la inserción de un titular con identificación duplicada
        /// </summary>
        [Fact]
        public void InsertDuplicateTest()
        {
            //Arrange
            Owner owner = new()
            {
                Name = "Prueba 1",
                IdentificationType = new() { Id = 1 },
                Identification = "123456789",
                AddressHome = "CL 19 # 20 - 21",
                AddressOffice = "CL 22 # 23 - 24",
                PhoneHome = "3571594682",
                PhoneOffice = "2864951753",
                Email = "prueba@prueba.com"
            };

            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _persistent.Insert(owner, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de un titular
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            Owner owner = new()
            {
                Id = 2,
                Name = "Prueba actualizar",
                IdentificationType = new() { Id = 1 },
                Identification = "987654321-1",
                AddressHome = "CL 25 # 26 - 27",
                AddressOffice = "CL 28 # 29 - 30",
                PhoneHome = "3571594682-1",
                PhoneOffice = "2864951753-1",
                Email = "prueba2@prueba.com"
            };
            Owner owner2 = new() { Id = 2 };

            //Act
            _ = _persistent.Update(owner, new() { Id = 1 });
            owner2 = _persistent.Read(owner2);

            //Assert
            Assert.NotEqual("David Santiago Baena Barreto", owner2.Name);
        }

        /// <summary>
        /// Prueba la actualización de un titular con error
        /// </summary>
        [Fact]
        public void UpdateWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Update(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un titular
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            Owner owner = new() { Id = 3 };
            Owner owner2 = new() { Id = 3 };

            //Act
            _ = _persistent.Delete(owner, new() { Id = 1 });
            owner2 = _persistent.Read(owner2);

            //Assert
            Assert.Equal(0, owner2.Id);
        }

        /// <summary>
        /// Prueba la eliminación de un titular con error
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