using Dal.Dto;
using Dal.Exceptions;
using Dal.Noti;
using Entities.Noti;
using Microsoft.Extensions.Configuration;

namespace Dal.Test.Noti
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de plantillas
    /// </summary>
    [Collection("Tests")]
    public class TemplateTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de persistencia en la base de datos
        /// </summary>
        private readonly PersistentTemplate _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public TemplateTest()
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
        /// Prueba la consulta de un listado de plantillas con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<Template> list = _persistent.List("idtemplate = 1", "idtemplate", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de plantillas con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.List("idplantilla = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una plantilla dado su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            Template template = new() { Id = 1 };

            //Act
            template = _persistent.Read(template);

            //Assert
            Assert.Equal("Plantilla de prueba", template.Name);
        }

        /// <summary>
        /// Prueba la consulta de una plantilla que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            Template template = new() { Id = 10 };

            //Act
            template = _persistent.Read(template);

            //Assert
            Assert.Equal(0, template.Id);
        }

        /// <summary>
        /// Prueba la consulta de una plantilla con error
        /// </summary>
        [Fact]
        public void ReadWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Read(null));
        }

        /// <summary>
        /// Prueba la inserción de una plantilla
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            Template template = new() { Name = "Plantilla insertada", Content = "<p>Prueba de una plantilla #{insertada}#</p>" };

            //Act
            template = _persistent.Insert(template, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, template.Id);
        }

        /// <summary>
        /// Prueba la inserción de una plantilla con error
        /// </summary>
        [Fact]
        public void InsertWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Insert(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de una plantilla
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            Template template = new() { Id = 2, Name = "Prueba actualizada", Content = "<h3>Contenido de una plantilla #{actualizada}#</h3>" };
            Template template2 = new() { Id = 2 };

            //Act
            _ = _persistent.Update(template, new() { Id = 1 });
            template2 = _persistent.Read(template2);

            //Assert
            Assert.NotEqual("Plantilla a actualizar", template2.Name);
        }

        /// <summary>
        /// Prueba la actualización de una plantilla con error
        /// </summary>
        [Fact]
        public void UpdateWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Update(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de una plantilla
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            Template template = new() { Id = 3 };
            Template template2 = new() { Id = 3 };

            //Act
            _ = _persistent.Delete(template, new() { Id = 1 });
            template2 = _persistent.Read(template2);

            //Assert
            Assert.Equal(0, template2.Id);
        }

        /// <summary>
        /// Prueba la eliminación de una plantilla con error
        /// </summary>
        [Fact]
        public void DeleteWithErrorTest()
        {
            //Assert
            Assert.Throws<PersistentException>(() => _persistent.Delete(null, new() { Id = 1 }));
        }
        #endregion
    }
}
