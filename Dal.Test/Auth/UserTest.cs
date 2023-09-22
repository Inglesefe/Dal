using Dal.Auth;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
using Microsoft.Extensions.Configuration;

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
        /// Prueba la consulta de un listado de usuarios con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<User> list = _persistent.List("iduser = 1", "name", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de usuarios con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.List("idusuario = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un usuario dado su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            User user = new() { Id = 1 };

            //Act
            user = _persistent.Read(user);

            //Assert
            Assert.Equal("leandrobaena@gmail.com", user.Login);
        }

        /// <summary>
        /// Prueba la consulta de un usuario que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            User user = new() { Id = 10 };

            //Act
            user = _persistent.Read(user);

            //Assert
            Assert.Equal(0, user.Id);
        }

        /// <summary>
        /// Prueba la consulta de un usuario con error
        /// </summary>
        [Fact]
        public void ReadWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Read(null));
        }

        /// <summary>
        /// Prueba la inserción de un usuario
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            User user = new() { Login = "insertado@prueba.com", Name = "Prueba 1", Active = true };

            //Act
            user = _persistent.Insert(user, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, user.Id);
        }

        /// <summary>
        /// Prueba la inserción de un usuario con login duplicado
        /// </summary>
        [Fact]
        public void InsertDuplicateTest()
        {
            //Arrange
            User user = new() { Login = "leandrobaena@gmail.com", Name = "Prueba insertar", Active = true };

            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _persistent.Insert(user, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de un usuario
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            User user = new() { Id = 2, Login = "otrologin@gmail.com", Name = "Prueba actualizar", Active = false };
            User user2 = new() { Id = 2 };

            //Act
            _ = _persistent.Update(user, new() { Id = 1 });
            user2 = _persistent.Read(user2);

            //Assert
            Assert.NotEqual("actualizame@gmail.com", user2.Name);
            Assert.False(user2.Active);
        }

        /// <summary>
        /// Prueba la actualización de un usuario con error
        /// </summary>
        [Fact]
        public void UpdateWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Update(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un usuario
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            User user = new() { Id = 3 };
            User user2 = new() { Id = 3 };

            //Act
            _ = _persistent.Delete(user, new() { Id = 1 });
            user2 = _persistent.Read(user2);

            //Assert
            Assert.Equal(0, user2.Id);
        }

        /// <summary>
        /// Prueba la eliminación de un usuario con error
        /// </summary>
        [Fact]
        public void DeleteWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Delete(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un usuario dado su login y contraseña
        /// </summary>
        [Fact]
        public void ReadByLoginAndPasswordTest()
        {
            //Arrange
            User user = new() { Login = "leandrobaena@gmail.com" };

            //Act
            user = _persistent.ReadByLoginAndPassword(user, "Prueba123");

            //Assert
            Assert.NotEqual(0, user.Id);
        }

        /// <summary>
        /// Prueba la consulta de un usuario que no existe dado su login y password
        /// </summary>
        [Fact]
        public void ReadByLoginAndPasswordWithErrorTest()
        {
            //Arrange
            User user = new() { Login = "actualizame@gmail.com" };

            //Act
            user = _persistent.ReadByLoginAndPassword(user, "Errada");

            //Assert
            Assert.Equal(0, user.Id);
        }

        /// <summary>
        /// Prueba la consulta de un usuario inactivo dado su login y password
        /// </summary>
        [Fact]
        public void ReadByLoginAndPasswordInactiveTest()
        {
            //Arrange
            User user = new() { Login = "inactivo@gmail.com" };

            //Act
            user = _persistent.ReadByLoginAndPassword(user, "Prueba123");

            //Assert
            Assert.Equal(0, user.Id);
        }

        /// <summary>
        /// Prueba la consulta de un usuario dado su login y contraseña con error
        /// </summary>
        [Fact]
        public void ReadByLoginAndPasswordWithError2Test()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.ReadByLoginAndPassword(null, ""));
        }

        /// <summary>
        /// Prueba la consulta de un usuario dado su login
        /// </summary>
        [Fact]
        public void ReadByLoginTest()
        {
            //Arrange
            User user = new() { Login = "leandrobaena@gmail.com" };

            //Act
            user = _persistent.ReadByLogin(user);

            //Assert
            Assert.NotEqual(0, user.Id);
        }

        /// <summary>
        /// Prueba la consulta de un usuario dado su login que no existe
        /// </summary>
        [Fact]
        public void ReadByLoginNonExistentTest()
        {
            //Arrange
            User user = new() { Login = "pepitoperez@inglesefe.com.co" };

            //Act
            user = _persistent.ReadByLogin(user);

            //Assert
            Assert.Equal(0, user.Id);
        }

        /// <summary>
        /// Prueba la consulta de un usuario dado su login con error
        /// </summary>
        [Fact]
        public void ReadByLoginWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.ReadByLogin(null));
        }

        /// <summary>
        /// Prueba la actualización de la contraseña de un usuario
        /// </summary>
        [Fact]
        public void UpdatePasswordTest()
        {
            //Arrange
            User user = new() { Id = 1, Login = "leandrobaena@gmail.com" };

            //Act
            _ = _persistent.UpdatePassword(user, "Prueba123", new() { Id = 1 });
            user = _persistent.ReadByLoginAndPassword(user, "Prueba123");

            //Assert
            Assert.NotEqual(0, user.Id);
        }

        /// <summary>
        /// Prueba la actualización de la contraseña de un usuario con error
        /// </summary>
        [Fact]
        public void UpdatePasswordWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.UpdatePassword(null, "", new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles de un usuario con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListRolesTest()
        {
            //Act
            ListResult<Role> list = _persistent.ListRoles("", "", 10, 0, new() { Id = 1 });

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles de un usuario con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListRolesWithErrorTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _persistent.ListRoles("idusuario = 1", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles no asignados a un usuario con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListNotRolesTest()
        {
            //Act
            ListResult<Role> list = _persistent.ListNotRoles("", "", 10, 0, new() { Id = 1 });

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles no asignados a un usuario con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListNotRolesWithErrorTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _persistent.ListNotRoles("idusuario = 1", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la inserción de un rol de un usuario
        /// </summary>
        [Fact]
        public void InsertRoleTest()
        {
            //Act
            Role role = _persistent.InsertRole(new() { Id = 4 }, new() { Id = 1 }, new() { Id = 1 });

            //Assert
            Assert.NotNull(role);
        }

        /// <summary>
        /// Prueba la inserción de un rol de un usuario duplicado
        /// </summary>
        [Fact]
        public void InsertRoleDuplicateTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _persistent.InsertRole(new() { Id = 1 }, new() { Id = 1 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un rol de un usuario
        /// </summary>
        [Fact]
        public void DeleteRoleTest()
        {
            //Act
            _ = _persistent.DeleteRole(new() { Id = 2 }, new() { Id = 1 }, new() { Id = 1 });
            ListResult<Role> list = _persistent.ListRoles("idrole = 2", "", 10, 0, new() { Id = 1 });

            //Assert
            Assert.Equal(0, list.Total);
        }

        /// <summary>
        /// Prueba la eliminación de un rol de un usuario con error
        /// </summary>
        [Fact]
        public void DeleteRoleWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.DeleteRole(null, new() { Id = 1 }, new() { Id = 1 }));
        }
        #endregion
    }
}