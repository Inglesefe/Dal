using Dal.Dto;
using Dal.Exceptions;
using Dal.Noti;
using Entities.Noti;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

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
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .AddEnvironmentVariables()
                .Build();
            _persistent = new(new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de plantillas con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void TemplateListTest()
        {
            ListResult<Template> list = _persistent.List("idtemplate = 1", "idtemplate", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de plantillas con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void TemplateListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idplantilla = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una plantilla dado su identificador
        /// </summary>
        [Fact]
        public void TemplateReadTest()
        {
            Template template = new() { Id = 1 };
            template = _persistent.Read(template);

            Assert.Equal("Plantilla de prueba", template.Name);
        }

        /// <summary>
        /// Prueba la consulta de una plantilla que no existe dado su identificador
        /// </summary>
        [Fact]
        public void TemplateReadNotFoundTest()
        {
            Template template = new() { Id = 10 };
            template = _persistent.Read(template);

            Assert.Equal(0, template.Id);
        }

        /// <summary>
        /// Prueba la inserción de una plantilla
        /// </summary>
        [Fact]
        public void TemplateInsertTest()
        {
            Template template = new() { Name = "Plantilla insertada", Content = "<p>Prueba de una plantilla #{insertada}#</p>" };
            template = _persistent.Insert(template, new() { Id = 1 });

            Assert.NotEqual(0, template.Id);
        }

        /// <summary>
        /// Prueba la actualización de una plantilla
        /// </summary>
        [Fact]
        public void TemplateUpdateTest()
        {
            Template template = new() { Id = 2, Name = "Prueba actualizada", Content = "<h3>Contenido de una plantilla #{actualizada}#</h3>" };
            _ = _persistent.Update(template, new() { Id = 1 });

            Template template2 = new() { Id = 2 };
            template2 = _persistent.Read(template2);

            Assert.NotEqual("Plantilla a actualizar", template2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de una plantilla
        /// </summary>
        [Fact]
        public void TemplateDeleteTest()
        {
            Template template = new() { Id = 3 };
            _ = _persistent.Delete(template, new() { Id = 1 });

            Template template2 = new() { Id = 3 };
            template2 = _persistent.Read(template2);

            Assert.Equal(0, template2.Id);
        }
        #endregion
    }
}
