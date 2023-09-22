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
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de los beneficiarios
        /// </summary>
        private readonly PersistentBeneficiary _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public BeneficiaryTest()
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
        /// Prueba la consulta de un listado de beneficiarios con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<Beneficiary> list = _persistent.List("idbeneficiary = 1", "idbeneficiary", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de beneficiarios con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.List("idbeneficiario = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un beneficiario dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            Beneficiary beneficiary = new() { Id = 1 };

            //Act
            beneficiary = _persistent.Read(beneficiary);

            //Assert
            Assert.Equal("Pedro Perez", beneficiary.Name);
        }

        /// <summary>
        /// Prueba la consulta de un beneficiario que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            Beneficiary beneficiary = new() { Id = 10 };

            //Act
            beneficiary = _persistent.Read(beneficiary);

            //Assert
            Assert.Equal(0, beneficiary.Id);
        }

        /// <summary>
        /// Prueba la consulta de un beneficiario con error
        /// </summary>
        [Fact]
        public void ReadWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Read(null));
        }

        /// <summary>
        /// Prueba la inserción de un beneficiario
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            Beneficiary beneficiary = new()
            {
                Name = "Prueba 1",
                IdentificationType = new() { Id = 1 },
                Identification = "963258741",
                Relationship = "Primo",
                Owner = new() { Id = 1 }
            };

            //Act
            beneficiary = _persistent.Insert(beneficiary, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, beneficiary.Id);
        }

        /// <summary>
        /// Prueba la inserción de un beneficiario con identificación duplicada
        /// </summary>
        [Fact]
        public void InsertDuplicateTest()
        {
            //Arrange
            Beneficiary beneficiary = new()
            {
                Name = "Prueba 2",
                IdentificationType = new() { Id = 1 },
                Identification = "111111111",
                Relationship = "Tio",
                Owner = new() { Id = 1 }
            };

            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _persistent.Insert(beneficiary, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de un beneficiario
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            Beneficiary beneficiary = new()
            {
                Id = 2,
                Name = "Prueba 2",
                IdentificationType = new() { Id = 1 },
                Identification = "444444444",
                Relationship = "Tio",
                Owner = new() { Id = 1 }
            };
            Beneficiary beneficiary2 = new() { Id = 2 };

            //Act
            _ = _persistent.Update(beneficiary, new() { Id = 1 });
            beneficiary2 = _persistent.Read(beneficiary2);

            //Assert
            Assert.NotEqual("Maria Martinez", beneficiary2.Name);
        }

        /// <summary>
        /// Prueba la actualización de un beneficiario con error
        /// </summary>
        [Fact]
        public void UpdateWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Update(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un beneficiario
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            Beneficiary beneficiary = new() { Id = 4 };
            Beneficiary beneficiary2 = new() { Id = 4 };

            //Act
            _ = _persistent.Delete(beneficiary, new() { Id = 1 });
            beneficiary2 = _persistent.Read(beneficiary2);

            //Assert
            Assert.Equal(0, beneficiary2.Id);
        }

        /// <summary>
        /// Prueba la eliminación de un beneficiario con error
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