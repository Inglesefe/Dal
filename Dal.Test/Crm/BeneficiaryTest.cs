using Dal.Crm;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Crm;
using Microsoft.Extensions.Configuration;

namespace Dal.Test.Crm
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de beneficiarios
    /// </summary>
    [Collection("Tests")]
    public class BeneficiaryTest
    {
        #region Attributes
        /// <summary>
        /// Configuraci�n de la aplicaci�n de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de los beneficiarios
        /// </summary>
        private readonly PersistentBeneficiary _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuraci�n de la prueba
        /// </summary>
        public BeneficiaryTest()
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
        /// Prueba la consulta de un listado de beneficiarios con filtros, ordenamientos y l�mite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            ListResult<Beneficiary> list = _persistent.List("idbeneficiary = 1", "idbeneficiary", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de beneficiarios con filtros, ordenamientos y l�mite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idbeneficiario = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un beneficiario dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            Beneficiary beneficiary = new() { Id = 1 };
            beneficiary = _persistent.Read(beneficiary);

            Assert.Equal("Pedro Perez", beneficiary.Name);
        }

        /// <summary>
        /// Prueba la consulta de un beneficiario que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            Beneficiary beneficiary = new() { Id = 10 };
            beneficiary = _persistent.Read(beneficiary);

            Assert.Equal(0, beneficiary.Id);
        }

        /// <summary>
        /// Prueba la inserci�n de un beneficiario
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            Beneficiary beneficiary = new()
            {
                Name = "Prueba 1",
                IdentificationType = new() { Id = 1 },
                Identification = "963258741",
                Relationship = "Primo",
                Owner = new() { Id = 1 }
            };
            beneficiary = _persistent.Insert(beneficiary, new() { Id = 1 });

            Assert.NotEqual(0, beneficiary.Id);
        }

        /// <summary>
        /// Prueba la inserci�n de un beneficiario con identificaci�n duplicada
        /// </summary>
        [Fact]
        public void InsertDuplicateTest()
        {
            Beneficiary beneficiary = new()
            {
                Name = "Prueba 2",
                IdentificationType = new() { Id = 1 },
                Identification = "111111111",
                Relationship = "Tio",
                Owner = new() { Id = 1 }
            };

            _ = Assert.Throws<PersistentException>(() => _persistent.Insert(beneficiary, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualizaci�n de un beneficiario
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            Beneficiary beneficiary = new()
            {
                Id = 2,
                Name = "Prueba 2",
                IdentificationType = new() { Id = 1 },
                Identification = "444444444",
                Relationship = "Tio",
                Owner = new() { Id = 1 }
            };
            _ = _persistent.Update(beneficiary, new() { Id = 1 });

            Beneficiary beneficiary2 = new() { Id = 2 };
            beneficiary2 = _persistent.Read(beneficiary2);

            Assert.NotEqual("Maria Martinez", beneficiary2.Name);
        }

        /// <summary>
        /// Prueba la eliminaci�n de un beneficiario
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            Beneficiary beneficiary = new() { Id = 4 };
            _ = _persistent.Delete(beneficiary, new() { Id = 1 });

            Beneficiary beneficiary2 = new() { Id = 4 };
            beneficiary2 = _persistent.Read(beneficiary2);

            Assert.Equal(0, beneficiary2.Id);
        }
        #endregion
    }
}