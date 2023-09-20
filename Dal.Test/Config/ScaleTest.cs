using Dal.Config;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Config;
using Microsoft.Extensions.Configuration;

namespace Dal.Test.Config
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de escalas
    /// </summary>
    [Collection("Tests")]
    public class ScaleTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de las escalas
        /// </summary>
        private readonly PersistentScale _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public ScaleTest()
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
        /// Prueba la consulta de un listado de escalas con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            ListResult<Scale> list = _persistent.List("idscale = 1", "name", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de escalas con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idescala = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una escala dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            Scale scale = new() { Id = 1 };
            scale = _persistent.Read(scale);

            Assert.Equal("Comision 1", scale.Name);
        }

        /// <summary>
        /// Prueba la consulta de una escala que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            Scale scale = new() { Id = 10 };
            scale = _persistent.Read(scale);

            Assert.Equal(0, scale.Id);
        }

        /// <summary>
        /// Prueba la inserción de una escala
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            Scale scale = new() { Code = "C4", Name = "Comision 4", Comission = 4000, Order = 4 };
            scale = _persistent.Insert(scale, new() { Id = 1 });

            Assert.NotEqual(0, scale.Id);
        }

        /// <summary>
        /// Prueba la actualización de una escala
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            Scale scale = new() { Id = 2, Code = "C5", Name = "Comision 5", Comission = 5000, Order = 5 };
            _ = _persistent.Update(scale, new() { Id = 1 });

            Scale scale2 = new() { Id = 2 };
            scale2 = _persistent.Read(scale2);

            Assert.NotEqual("Comision 2", scale2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de una escala
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            Scale scale = new() { Id = 3 };
            _ = _persistent.Delete(scale, new() { Id = 1 });

            Scale scale2 = new() { Id = 3 };
            scale2 = _persistent.Read(scale2);

            Assert.Equal(0, scale2.Id);
        }
        #endregion
    }
}
