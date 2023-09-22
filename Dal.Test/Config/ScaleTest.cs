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
        /// Prueba la consulta de un listado de escalas con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<Scale> list = _persistent.List("idscale = 1", "name", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de escalas con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.List("idescala = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una escala dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            Scale scale = new() { Id = 1 };

            //Act
            scale = _persistent.Read(scale);

            //Assert
            Assert.Equal("Comision 1", scale.Name);
        }

        /// <summary>
        /// Prueba la consulta de una escala que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            Scale scale = new() { Id = 10 };

            //Act
            scale = _persistent.Read(scale);

            //Assert
            Assert.Equal(0, scale.Id);
        }

        /// <summary>
        /// Prueba la consulta de una escala con error
        /// </summary>
        [Fact]
        public void ReadWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Read(null));
        }

        /// <summary>
        /// Prueba la inserción de una escala
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            Scale scale = new() { Code = "C4", Name = "Comision 4", Comission = 4000, Order = 4 };

            //Act
            scale = _persistent.Insert(scale, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, scale.Id);
        }

        /// <summary>
        /// Prueba la inserción de una escala con error
        /// </summary>
        [Fact]
        public void InsertWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Insert(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de una escala
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            Scale scale = new() { Id = 2, Code = "C5", Name = "Comision 5", Comission = 5000, Order = 5 };
            Scale scale2 = new() { Id = 2 };

            //Act
            _ = _persistent.Update(scale, new() { Id = 1 });
            scale2 = _persistent.Read(scale2);

            //Assert
            Assert.NotEqual("Comision 2", scale2.Name);
        }

        /// <summary>
        /// Prueba la actualización de una escala con error
        /// </summary>
        [Fact]
        public void UpdateWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Update(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de una escala
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            Scale scale = new() { Id = 3 };
            Scale scale2 = new() { Id = 3 };

            //Act
            _ = _persistent.Delete(scale, new() { Id = 1 });
            scale2 = _persistent.Read(scale2);

            //Assert
            Assert.Equal(0, scale2.Id);
        }

        /// <summary>
        /// Prueba la eliminación de una escala con error
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
