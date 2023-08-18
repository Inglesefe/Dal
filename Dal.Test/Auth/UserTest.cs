using Dal.Auth;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Dal.Test.Auth
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de usuarios
    /// </summary>
    [Collection("Tests")]
    public class UserTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de persistencia en la base de datos
        /// </summary>
        private readonly PersistentUser _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public UserTest()
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
        /// Prueba la consulta de un listado de usuarios con filtros, ordenamientos y límite
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void UserListTest()
        {
            ListResult<User> list = _persistent.List("iduser = 1", "name", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de usuarios con filtros, ordenamientos y límite y con errores
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void UserListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idusuario = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un usuario dado su identificador
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void UserReadTest()
        {
            User user = new() { Id = 1 };
            user = _persistent.Read(user);

            Assert.Equal("leandrobaena@gmail.com", user.Login);
        }

        /// <summary>
        /// Prueba la consulta de un usuario que no existe dado su identificador
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void UserReadNotFoundTest()
        {
            User user = new() { Id = 10 };
            user = _persistent.Read(user);

            Assert.Null(user);
        }

        /// <summary>
        /// Prueba la inserción de un usuario
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void UserInsertTest()
        {
            User user = new() { Login = "insertado@prueba.com", Name = "Prueba 1", Active = true };
            user = _persistent.Insert(user, new() { Id = 1 });

            Assert.NotEqual(0, user.Id);
        }

        /// <summary>
        /// Prueba la inserción de un usuario con login duplicado
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void UserInsertDuplicateTest()
        {
            User user = new() { Login = "leandrobaena@gmail.com", Name = "Prueba insertar", Active = true };

            _ = Assert.Throws<PersistentException>(() => _persistent.Insert(user, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de un usuario
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void UserUpdateTest()
        {
            User user = new() { Id = 2, Login = "otrologin@gmail.com", Name = "Prueba actualizar", Active = false };
            _ = _persistent.Update(user, new() { Id = 1 });

            User user2 = new() { Id = 2 };
            user2 = _persistent.Read(user2);

            Assert.NotEqual("actualizame@gmail.com", user2.Name);
            Assert.False(user2.Active);
        }

        /// <summary>
        /// Prueba la eliminación de un usuario
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void UserDeleteTest()
        {
            User user = new() { Id = 3 };
            _ = _persistent.Delete(user, new() { Id = 1 });

            User user2 = new() { Id = 3 };
            user2 = _persistent.Read(user2);

            Assert.Null(user2);
        }

        /// <summary>
        /// Prueba la consulta de un usuario dado su login y contraseña
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void UserReadByLoginAndPasswordTest()
        {
            User user = new() { Login = "leandrobaena@gmail.com" };
            user = _persistent.ReadByLoginAndPassword(user, "Prueba123");

            Assert.NotNull(user);
        }

        /// <summary>
        /// Prueba la consulta de un usuario que no existe dado su login y password
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void UserReadByLoginAndPasswordWithErrorTest()
        {
            User user = new() { Login = "actualizame@gmail.com" };
            user = _persistent.ReadByLoginAndPassword(user, "Errada");

            Assert.Null(user);
        }

        /// <summary>
        /// Prueba la consulta de un usuario dado su login
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void UserReadByLoginTest()
        {
            User user = new() { Login = "leandrobaena@gmail.com" };
            user = _persistent.ReadByLogin(user);

            Assert.NotNull(user);
        }

        /// <summary>
        /// Prueba la consulta de un usuario inactivo dado su login y password
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void UserReadByLoginAndPasswordInactiveTest()
        {
            User user = new() { Login = "inactivo@gmail.com" };
            user = _persistent.ReadByLoginAndPassword(user, "Prueba123");

            Assert.Null(user);
        }

        /// <summary>
        /// Prueba la actualización de la contraseña de un usuario
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void UserUpdatePasswordTest()
        {
            User user = new() { Id = 1, Login = "leandrobaena@gmail.com" };
            _ = _persistent.UpdatePassword(user, "Prueba123", new() { Id = 1 });

            user = _persistent.ReadByLoginAndPassword(user, "Prueba123");

            Assert.NotNull(user);
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles de un usuario con filtros, ordenamientos y límite
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void UserListRolesTest()
        {
            ListResult<Role> list = _persistent.ListRoles("", "", 10, 0, new() { Id = 1 });

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles de un usuario con filtros, ordenamientos y límite y con errores
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void UserListRolesWithErrorTest()
        {
            _ = Assert.Throws<PersistentException>(() => _persistent.ListRoles("idusuario = 1", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles no asignados a un usuario con filtros, ordenamientos y límite
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void UserListNotRolesTest()
        {
            ListResult<Role> list = _persistent.ListNotRoles("", "", 10, 0, new() { Id = 1 });

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la inserción de un rol de un usuario
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void UserInsertRoleTest()
        {
            Role role = _persistent.InsertRole(new() { Id = 4 }, new() { Id = 1 }, new() { Id = 1 });

            Assert.NotNull(role);
        }

        /// <summary>
        /// Prueba la inserción de un rol de un usuario duplicado
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void UserInsertRoleDuplicateTest()
        {
            _ = Assert.Throws<PersistentException>(() => _persistent.InsertRole(new() { Id = 1 }, new() { Id = 1 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un rol de un usuario
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void UserDeleteRoleTest()
        {
            _ = _persistent.DeleteRole(new() { Id = 2 }, new() { Id = 1 }, new() { Id = 1 });
            ListResult<Role> list = _persistent.ListRoles("r.idrole = 2", "", 10, 0, new() { Id = 1 });

            Assert.Equal(0, list.Total);
        }
        #endregion
    }
}