using Dal.Cont;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Cont;
using Microsoft.Extensions.Configuration;

namespace Dal.Test.Cont
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de consecutivos de consecutivo
    /// </summary>
    [Collection("Tests")]
    public class ConsecutiveTypeTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de los consecutivos de consecutivo
        /// </summary>
        private readonly PersistentConsecutiveType _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public ConsecutiveTypeTest()
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
        /// Prueba la consulta de un listado de consecutivos de consecutivo con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            ListResult<ConsecutiveType> list = _persistent.List("idconsecutivetype = 1", "name", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de consecutivos de consecutivo con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idconsecutivoconsecutivo = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un tipo de consecutivo dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            ConsecutiveType type = new() { Id = 1 };
            type = _persistent.Read(type);

            Assert.Equal("Recibos de caja", type.Name);
        }

        /// <summary>
        /// Prueba la consulta de un tipo de consecutivo que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            ConsecutiveType type = new() { Id = 10 };
            type = _persistent.Read(type);

            Assert.Equal(0, type.Id);
        }

        /// <summary>
        /// Prueba la inserción de un tipo de consecutivo
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            ConsecutiveType type = new() { Name = "Prueba 1" };
            type = _persistent.Insert(type, new() { Id = 1 });

            Assert.NotEqual(0, type.Id);
        }

        /// <summary>
        /// Prueba la actualización de un tipo de consecutivo
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            ConsecutiveType type = new() { Id = 2, Name = "Prueba actualizar" };
            _ = _persistent.Update(type, new() { Id = 1 });

            ConsecutiveType type2 = new() { Id = 2 };
            type2 = _persistent.Read(type2);

            Assert.NotEqual("Registro oficial", type2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de un tipo de consecutivo
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            ConsecutiveType type = new() { Id = 3 };
            _ = _persistent.Delete(type, new() { Id = 1 });

            ConsecutiveType type2 = new() { Id = 3 };
            type2 = _persistent.Read(type2);

            Assert.Equal(0, type2.Id);
        }
        #endregion
    }
}