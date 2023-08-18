using Dal.Auth;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Dal.Test.Auth
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de aplicaciones
    /// </summary>
    [Collection("Tests")]
    public class ApplicationTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de las aplicaciones
        /// </summary>
        private readonly PersistentApplication _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public ApplicationTest()
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
        /// Prueba la consulta de un listado de aplicaciones con filtros, ordenamientos y límite
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void ApplicationListTest()
        {
            ListResult<Application> list = _persistent.List("idapplication = 1", "name", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones con filtros, ordenamientos y límite y con errores
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void ApplicationListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idaplicacion = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una aplicación dada su identificador
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void ApplicationReadTest()
        {
            Application application = new() { Id = 1 };
            application = _persistent.Read(application);

            Assert.Equal("Autenticacion", application.Name);
        }

        /// <summary>
        /// Prueba la consulta de una aplicación que no existe dado su identificador
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void ApplicationReadNotFoundTest()
        {
            Application application = new() { Id = 10 };
            application = _persistent.Read(application);

            Assert.Null(application);
        }

        /// <summary>
        /// Prueba la inserción de una aplicación
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void ApplicationInsertTest()
        {
            Application application = new() { Name = "Prueba 1" };
            application = _persistent.Insert(application, new() { Id = 1 });

            Assert.NotEqual(0, application.Id);
        }

        /// <summary>
        /// Prueba la inserción de una aplicación con nombre duplicado
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void ApplicationInsertDuplicateTest()
        {
            Application application = new() { Name = "Autenticacion" };

            _ = Assert.Throws<PersistentException>(() => _persistent.Insert(application, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de una aplicación
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void ApplicationUpdateTest()
        {
            Application application = new() { Id = 2, Name = "Prueba actualizar" };
            _ = _persistent.Update(application, new() { Id = 1 });

            Application application2 = new() { Id = 2 };
            application2 = _persistent.Read(application2);

            Assert.NotEqual("Actualizame", application2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de una aplicación
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void ApplicationDeleteTest()
        {
            Application application = new() { Id = 3 };
            _ = _persistent.Delete(application, new() { Id = 1 });

            Application application2 = new() { Id = 3 };
            application2 = _persistent.Read(application2);

            Assert.Null(application2);
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles de una aplicación con filtros, ordenamientos y límite
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void ApplicationListRolesTest()
        {
            ListResult<Role> list = _persistent.ListRoles("", "r.idrole asc", 10, 0, new() { Id = 1 });

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles de una aplicación con filtros, ordenamientos y límite y con errores
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void ApplicationListRolesWithErrorTest()
        {
            _ = Assert.Throws<PersistentException>(() => _persistent.ListRoles("idaplicación = 1", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles no asignados a una aplicación con filtros, ordenamientos y límite
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void ApplicationListNotRolesTest()
        {
            ListResult<Role> list = _persistent.ListNotRoles("", "", 10, 0, new() { Id = 1 });

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la inserción de un rol de una aplicación
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void ApplicationInsertRoleTest()
        {
            Role role = _persistent.InsertRole(new() { Id = 4 }, new() { Id = 1 }, new() { Id = 1 });

            Assert.NotNull(role);
        }

        /// <summary>
        /// Prueba la inserción de un rol de una aplicación duplicado
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void ApplicationInsertRoleDuplicateTest()
        {
            _ = Assert.Throws<PersistentException>(() => _persistent.InsertRole(new() { Id = 1 }, new() { Id = 1 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un rol de una aplicación
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void ApplicationDeleteRoleTest()
        {
            _ = _persistent.DeleteRole(new() { Id = 2 }, new() { Id = 1 }, new() { Id = 1 });
            ListResult<Role> list = _persistent.ListRoles("r.idrole = 2", "", 10, 0, new() { Id = 1 });

            Assert.Equal(0, list.Total);
        }
        #endregion
    }
}