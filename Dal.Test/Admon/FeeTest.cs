using Dal.Admon;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Admon;
using Microsoft.Extensions.Configuration;

namespace Dal.Test.Admon
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de cuotas de matr�culas
    /// </summary>
    [Collection("Tests")]
    public class FeeTest
    {
        #region Attributes
        /// <summary>
        /// Configuraci�n de la aplicaci�n de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de las cuotas de matr�culas
        /// </summary>
        private readonly PersistentFee _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuraci�n de la prueba
        /// </summary>
        public FeeTest()
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
        /// Prueba la consulta de un listado de cuotas de matr�culas con filtros, ordenamientos y l�mite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            ListResult<Fee> list = _persistent.List("idfee = 1", "", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de cuotas de matr�culas con filtros, ordenamientos y l�mite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idcuota = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una cuota de matr�cula dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            Fee fee = new() { Id = 1 };
            fee = _persistent.Read(fee);

            Assert.Equal(1000, fee.Value);
        }

        /// <summary>
        /// Prueba la consulta de una cuota de matr�cula que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            Fee fee = new() { Id = 10 };
            fee = _persistent.Read(fee);

            Assert.Equal(0, fee.Id);
        }

        /// <summary>
        /// Prueba la inserci�n de una cuota de matr�cula
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            Fee fee = new() { Registration = new() { Id = 1 }, Value = 4000, Number = 4, IncomeType = new() { Id = 1 }, DueDate = DateTime.Now };
            fee = _persistent.Insert(fee, new() { Id = 1 });

            Assert.NotEqual(0, fee.Id);
        }

        /// <summary>
        /// Prueba la inserci�n de una cuota de matr�cula con identificaci�n duplicada
        /// </summary>
        [Fact]
        public void InsertDuplicateTest()
        {
            Fee fee = new() { Registration = new() { Id = 1 }, Value = 6000, Number = 1, IncomeType = new() { Id = 1 }, DueDate = DateTime.Now };

            _ = Assert.Throws<PersistentException>(() => _persistent.Insert(fee, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualizaci�n de una cuota de matr�cula
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            Fee fee = new() { Id = 2, Registration = new() { Id = 1 }, Value = 5000, Number = 2, IncomeType = new() { Id = 1 }, DueDate = DateTime.Now };
            _ = _persistent.Update(fee, new() { Id = 1 });

            Fee fee2 = new() { Id = 2 };
            fee2 = _persistent.Read(fee2);

            Assert.NotEqual(2000, fee2.Value);
        }

        /// <summary>
        /// Prueba la eliminaci�n de una cuota de matr�cula
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            Fee fee = new() { Id = 3 };
            _ = _persistent.Delete(fee, new() { Id = 1 });

            Fee fee2 = new() { Id = 3 };
            fee2 = _persistent.Read(fee2);

            Assert.Equal(0, fee2.Id);
        }
        #endregion
    }
}