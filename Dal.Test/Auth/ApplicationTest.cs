using Dal.Auth;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
using Microsoft.Extensions.Configuration;

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
            _persistent = new(_configuration.GetConnectionString("golden") ?? "");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            ListResult<Application> list = _persistent.List("idapplication = 1", "name", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idaplicacion = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una aplicación dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            Application application = new() { Id = 1 };
            application = _persistent.Read(application);

            Assert.Equal("Autenticacion", application.Name);
        }

        /// <summary>
        /// Prueba la consulta de una aplicación que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            Application application = new() { Id = 10 };
            application = _persistent.Read(application);

            Assert.Equal(0, application.Id);
        }

        /// <summary>
        /// Prueba la inserción de una aplicación
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            Application application = new() { Name = "Prueba 1" };
            application = _persistent.Insert(application, new() { Id = 1 });

            Assert.NotEqual(0, application.Id);
        }

        /// <summary>
        /// Prueba la inserción de una aplicación con nombre duplicado
        /// </summary>
        [Fact]
        public void InsertDuplicateTest()
        {
            Application application = new() { Name = "Autenticacion" };

            _ = Assert.Throws<PersistentException>(() => _persistent.Insert(application, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de una aplicación
        /// </summary>
        [Fact]
        public void UpdateTest()
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
        [Fact]
        public void DeleteTest()
        {
            Application application = new() { Id = 3 };
            _ = _persistent.Delete(application, new() { Id = 1 });

            Application application2 = new() { Id = 3 };
            application2 = _persistent.Read(application2);

            Assert.Equal(0, application2.Id);
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles de una aplicación con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListRolesTest()
        {
            ListResult<Role> list = _persistent.ListRoles("", "idrole asc", 10, 0, new() { Id = 1 });

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles de una aplicación con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListRolesWithErrorTest()
        {
            _ = Assert.Throws<PersistentException>(() => _persistent.ListRoles("idaplicación = 1", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles no asignados a una aplicación con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListNotRolesTest()
        {
            ListResult<Role> list = _persistent.ListNotRoles("", "", 10, 0, new() { Id = 1 });

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la inserción de un rol de una aplicación
        /// </summary>
        [Fact]
        public void InsertRoleTest()
        {
            Role role = _persistent.InsertRole(new() { Id = 4 }, new() { Id = 1 }, new() { Id = 1 });

            Assert.NotEqual(0, role.Id);
        }

        /// <summary>
        /// Prueba la inserción de un rol de una aplicación duplicado
        /// </summary>
        [Fact]
        public void InsertRoleDuplicateTest()
        {
            _ = Assert.Throws<PersistentException>(() => _persistent.InsertRole(new() { Id = 1 }, new() { Id = 1 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un rol de una aplicación
        /// </summary>
        [Fact]
        public void DeleteRoleTest()
        {
            _ = _persistent.DeleteRole(new() { Id = 2 }, new() { Id = 1 }, new() { Id = 1 });
            ListResult<Role> list = _persistent.ListRoles("idrole = 2", "", 10, 0, new() { Id = 1 });

            Assert.Equal(0, list.Total);
        }
        #endregion
    }
}