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
            ListResult<Owner> list = _persistent.List("idowner = 1", "idowner", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de titulares con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idtitular = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un titular dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            Owner owner = new() { Id = 1 };
            owner = _persistent.Read(owner);

            Assert.Equal("Leandro Baena Torres", owner.Name);
        }

        /// <summary>
        /// Prueba la consulta de un titular que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            Owner owner = new() { Id = 10 };
            owner = _persistent.Read(owner);

            Assert.Equal(0, owner.Id);
        }

        /// <summary>
        /// Prueba la inserción de un titular
        /// </summary>
        [Fact]
        public void InsertTest()
        {
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
            owner = _persistent.Insert(owner, new() { Id = 1 });

            Assert.NotEqual(0, owner.Id);
        }

        /// <summary>
        /// Prueba la inserción de un titular con identificación duplicada
        /// </summary>
        [Fact]
        public void InsertDuplicateTest()
        {
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

            _ = Assert.Throws<PersistentException>(() => _persistent.Insert(owner, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de un titular
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
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
            _ = _persistent.Update(owner, new() { Id = 1 });

            Owner owner2 = new() { Id = 2 };
            owner2 = _persistent.Read(owner2);

            Assert.NotEqual("David Santiago Baena Barreto", owner2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de un titular
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            Owner owner = new() { Id = 3 };
            _ = _persistent.Delete(owner, new() { Id = 1 });

            Owner owner2 = new() { Id = 3 };
            owner2 = _persistent.Read(owner2);

            Assert.Equal(0, owner2.Id);
        }
        #endregion
    }
}