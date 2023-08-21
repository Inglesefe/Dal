using Dal.Auth;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Dal.Test.Auth
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de roles
    /// </summary>
    [Collection("Tests")]
    public class RoleTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de persistencia los roles
        /// </summary>
        private readonly PersistentRole _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public RoleTest()
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
        /// Prueba la consulta de un listado de roles con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void RoleListTest()
        {
            ListResult<Role> list = _persistent.List("idrole = 1", "name", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void RoleListWithErrorTest()
        {
            _ = Assert.Throws<PersistentException>(() => _persistent.List("idrol = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un rol dado su identificador
        /// </summary>
        [Fact]
        public void RoleReadTest()
        {
            Role role = new() { Id = 1 };
            role = _persistent.Read(role);

            Assert.Equal("Administradores", role.Name);
        }

        /// <summary>
        /// Prueba la consulta de un rol que no existe dado su identificador
        /// </summary>
        [Fact]
        public void RoleReadNotFoundTest()
        {
            Role role = new() { Id = 10 };
            role = _persistent.Read(role);

            Assert.Equal(0, role.Id);
        }

        /// <summary>
        /// Prueba la inserción de un rol
        /// </summary>
        [Fact]
        public void RoleInsertTest()
        {
            Role role = new() { Name = "Prueba insercion" };
            role = _persistent.Insert(role, new() { Id = 1 });

            Assert.NotEqual(0, role.Id);
        }

        /// <summary>
        /// Prueba la inserción de un rol con nombre duplicado
        /// </summary>
        [Fact]
        public void RoleInsertDuplicateTest()
        {
            Role role = new() { Name = "Administradores" };

            _ = Assert.Throws<PersistentException>(() => _persistent.Insert(role, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de un rol
        /// </summary>
        [Fact]
        public void RoleUpdateTest()
        {
            Role role = new() { Id = 2, Name = "Prueba actualizar" };
            _ = _persistent.Update(role, new() { Id = 1 });

            Role role2 = new() { Id = 2 };
            role2 = _persistent.Read(role2);

            Assert.NotEqual("Actualizame", role2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de un rol
        /// </summary>
        [Fact]
        public void RoleDeleteTest()
        {
            Role role = new() { Id = 3 };
            _ = _persistent.Delete(role, new() { Id = 1 });

            Role role2 = new() { Id = 3 };
            role2 = _persistent.Read(role2);

            Assert.Equal(0, role2.Id);
        }

        /// <summary>
        /// Prueba la consulta de un listado de usuarios de un rol con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void RoleListUsersTest()
        {
            ListResult<User> list = _persistent.ListUsers("", "", 10, 0, new() { Id = 1 });

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de usuarios de un rol con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void RoleListUsersWithErrorTest()
        {
            _ = Assert.Throws<PersistentException>(() => _persistent.ListUsers("idusuario = 1", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de usuarios no asignados a un rol con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void RoleListNotUsersTest()
        {
            ListResult<User> list = _persistent.ListNotUsers("", "", 10, 0, new() { Id = 1 });

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la inserción de un usuario de un rol
        /// </summary>
        [Fact]
        public void RoleInsertUserTest()
        {
            User role = _persistent.InsertUser(new() { Id = 2 }, new() { Id = 4 }, new() { Id = 1 });

            Assert.NotNull(role);
        }

        /// <summary>
        /// Prueba la inserción de un usuario de un rol duplicado
        /// </summary>
        [Fact]
        public void RoleInsertUserDuplicateTest()
        {
            _ = Assert.Throws<PersistentException>(() => _persistent.InsertUser(new() { Id = 2 }, new() { Id = 1 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un usuario de un rol
        /// </summary>
        [Fact]
        public void RoleDeleteUserTest()
        {
            _ = _persistent.DeleteUser(new() { Id = 2 }, new() { Id = 2 }, new() { Id = 1 });
            ListResult<User> list = _persistent.ListUsers("u.iduser = 2", "", 10, 0, new() { Id = 2 });
            Assert.Equal(0, list.Total);
        }


        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones de un rol con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void RoleListApplicationsTest()
        {
            ListResult<Application> list = _persistent.ListApplications("", "", 10, 0, new() { Id = 1 });

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones de un rol con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void RoleListApplicationsWithErrorTest()
        {
            _ = Assert.Throws<PersistentException>(() => _persistent.ListApplications("idaplicacion = 1", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones no asignadas a un rol con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void RoleListNotApplicationsTest()
        {
            ListResult<Application> list = _persistent.ListNotApplications("", "", 10, 0, new() { Id = 1 });

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la inserción de una aplicación de un rol
        /// </summary>
        [Fact]
        public void RoleInsertApplicationTest()
        {
            Application application = _persistent.InsertApplication(new() { Id = 2 }, new() { Id = 4 }, new() { Id = 1 });

            Assert.NotNull(application);
        }

        /// <summary>
        /// Prueba la inserción de una aplicación de un rol duplicado
        /// </summary>
        [Fact]
        public void RoleInsertApplicationDuplicateTest()
        {
            _ = Assert.Throws<PersistentException>(() => _persistent.InsertUser(new() { Id = 2 }, new() { Id = 1 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de una aplicación de un rol
        /// </summary>
        [Fact]
        public void RoleDeleteApplicationTest()
        {
            _ = _persistent.DeleteApplication(new() { Id = 2 }, new() { Id = 2 }, new() { Id = 1 });
            ListResult<Application> list = _persistent.ListApplications("a.idapplication = 2", "", 10, 0, new() { Id = 2 });

            Assert.Equal(0, list.Total);
        }
        #endregion
    }
}