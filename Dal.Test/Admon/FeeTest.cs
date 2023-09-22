using Dal.Admon;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Admon;
using Microsoft.Extensions.Configuration;

namespace Dal.Test.Admon
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de cuotas de matrículas
    /// </summary>
    [Collection("Tests")]
    public class FeeTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de las cuotas de matrículas
        /// </summary>
        private readonly PersistentFee _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public FeeTest()
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
        /// Prueba la consulta de un listado de cuotas de matrículas con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<Fee> list = _persistent.List("idfee = 1", "", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de cuotas de matrículas con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.List("idcuota = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una cuota de matrícula dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            Fee fee = new() { Id = 1 };

            //Act
            fee = _persistent.Read(fee);

            //Assert
            Assert.Equal(1000, fee.Value);
        }

        /// <summary>
        /// Prueba la consulta de una cuota de matrícula que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            Fee fee = new() { Id = 10 };

            //Act
            fee = _persistent.Read(fee);

            //Assert
            Assert.Equal(0, fee.Id);
        }

        /// <summary>
        /// Prueba la consulta de una cuota de matrícula con error
        /// </summary>
        [Fact]
        public void ReadWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Read(null));
        }

        /// <summary>
        /// Prueba la inserción de una cuota de matrícula
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            Fee fee = new() { Registration = new() { Id = 1 }, Value = 4000, Number = 4, IncomeType = new() { Id = 1 }, DueDate = DateTime.Now };

            //Act
            fee = _persistent.Insert(fee, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, fee.Id);
        }

        /// <summary>
        /// Prueba la inserción de una cuota de matrícula con identificación duplicada
        /// </summary>
        [Fact]
        public void InsertDuplicateTest()
        {
            //Assert
            Fee fee = new() { Registration = new() { Id = 1 }, Value = 6000, Number = 1, IncomeType = new() { Id = 1 }, DueDate = DateTime.Now };

            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _persistent.Insert(fee, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de una cuota de matrícula
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Assert
            Fee fee = new() { Id = 2, Registration = new() { Id = 1 }, Value = 5000, Number = 2, IncomeType = new() { Id = 1 }, DueDate = DateTime.Now };
            Fee fee2 = new() { Id = 2 };

            //Assert
            _ = _persistent.Update(fee, new() { Id = 1 });
            fee2 = _persistent.Read(fee2);

            //Assert
            Assert.NotEqual(2000, fee2.Value);
        }

        /// <summary>
        /// Prueba la actualización de una cuota de matrícula con error
        /// </summary>
        [Fact]
        public void UpdateWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Update(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de una cuota de matrícula
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            Fee fee = new() { Id = 3 };
            Fee fee2 = new() { Id = 3 };

            //Act
            _ = _persistent.Delete(fee, new() { Id = 1 });
            fee2 = _persistent.Read(fee2);

            //Assert
            Assert.Equal(0, fee2.Id);
        }

        /// <summary>
        /// Prueba la eliminación de una cuota de matrícula con error
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