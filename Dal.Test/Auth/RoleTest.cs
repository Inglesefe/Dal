using Dal.Auth;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
using Microsoft.Extensions.Configuration;

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
        /// Prueba la consulta de un listado de roles con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<Role> list = _persistent.List("idrole = 1", "name", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _persistent.List("idrol = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un rol dado su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            Role role = new() { Id = 1 };

            //Act
            role = _persistent.Read(role);

            //Assert
            Assert.Equal("Administradores", role.Name);
        }

        /// <summary>
        /// Prueba la consulta de un rol que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            Role role = new() { Id = 10 };

            //Act
            role = _persistent.Read(role);

            //Assert
            Assert.Equal(0, role.Id);
        }

        /// <summary>
        /// Prueba la consulta de un rol dado su identificador con error
        /// </summary>
        [Fact]
        public void ReadWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Read(null));
        }

        /// <summary>
        /// Prueba la inserción de un rol
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            Role role = new() { Name = "Prueba insercion" };

            //Act
            role = _persistent.Insert(role, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, role.Id);
        }

        /// <summary>
        /// Prueba la inserción de un rol con nombre duplicado
        /// </summary>
        [Fact]
        public void InsertDuplicateTest()
        {
            //Arrange
            Role role = new() { Name = "Administradores" };

            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _persistent.Insert(role, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de un rol
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            Role role = new() { Id = 2, Name = "Prueba actualizar" };
            Role role2 = new() { Id = 2 };

            //Act
            _ = _persistent.Update(role, new() { Id = 1 });
            role2 = _persistent.Read(role2);

            //Assert
            Assert.NotEqual("Actualizame", role2.Name);
        }

        /// <summary>
        /// Prueba la actualización de un rol con eror
        /// </summary>
        [Fact]
        public void UpdateWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Update(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un rol
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            Role role = new() { Id = 3 };
            Role role2 = new() { Id = 3 };

            //Act
            _ = _persistent.Delete(role, new() { Id = 1 });
            role2 = _persistent.Read(role2);

            //Assert
            Assert.Equal(0, role2.Id);
        }

        /// <summary>
        /// Prueba la eliminación de un rol con error
        /// </summary>
        [Fact]
        public void DeleteWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Delete(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de usuarios de un rol con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListUsersTest()
        {
            //Act
            ListResult<User> list = _persistent.ListUsers("", "", 10, 0, new() { Id = 1 });

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de usuarios de un rol con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListUsersWithErrorTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _persistent.ListUsers("idusuario = 1", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de usuarios no asignados a un rol con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListNotUsersTest()
        {
            //Act
            ListResult<User> list = _persistent.ListNotUsers("", "", 10, 0, new() { Id = 1 });

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de usuarios no asociados a un rol con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListNotUsersWithErrorTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _persistent.ListNotUsers("idusuario = 1", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la inserción de un usuario de un rol
        /// </summary>
        [Fact]
        public void InsertUserTest()
        {
            //Act
            User role = _persistent.InsertUser(new() { Id = 2 }, new() { Id = 4 }, new() { Id = 1 });

            //Assert
            Assert.NotNull(role);
        }

        /// <summary>
        /// Prueba la inserción de un usuario de un rol duplicado
        /// </summary>
        [Fact]
        public void InsertUserDuplicateTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _persistent.InsertUser(new() { Id = 2 }, new() { Id = 1 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un usuario de un rol
        /// </summary>
        [Fact]
        public void DeleteUserTest()
        {
            //Act
            _ = _persistent.DeleteUser(new() { Id = 2 }, new() { Id = 2 }, new() { Id = 1 });
            ListResult<User> list = _persistent.ListUsers("iduser = 2", "", 10, 0, new() { Id = 2 });

            //Assert
            Assert.Equal(0, list.Total);
        }

        /// <summary>
        /// Prueba la eliminación de un usuario de un rol con error
        /// </summary>
        [Fact]
        public void DeleteUserWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.DeleteUser(null, new() { Id = 1 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones de un rol con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListApplicationsTest()
        {
            //Act
            ListResult<Application> list = _persistent.ListApplications("", "", 10, 0, new() { Id = 1 });

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones de un rol con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListApplicationsWithErrorTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _persistent.ListApplications("idaplicacion = 1", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones no asignadas a un rol con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListNotApplicationsTest()
        {
            //Act
            ListResult<Application> list = _persistent.ListNotApplications("", "", 10, 0, new() { Id = 1 });

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones no asociadas a un rol con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListNotApplicationsWithErrorTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _persistent.ListNotApplications("idaplicacion = 1", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la inserción de una aplicación de un rol
        /// </summary>
        [Fact]
        public void InsertApplicationTest()
        {
            //Act
            Application application = _persistent.InsertApplication(new() { Id = 2 }, new() { Id = 4 }, new() { Id = 1 });

            //Assert
            Assert.NotNull(application);
        }

        /// <summary>
        /// Prueba la inserción de una aplicación de un rol duplicado
        /// </summary>
        [Fact]
        public void InsertApplicationDuplicateTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _persistent.InsertApplication(new() { Id = 2 }, new() { Id = 1 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de una aplicación de un rol
        /// </summary>
        [Fact]
        public void DeleteApplicationTest()
        {
            //Act
            _ = _persistent.DeleteApplication(new() { Id = 2 }, new() { Id = 2 }, new() { Id = 1 });
            ListResult<Application> list = _persistent.ListApplications("idapplication = 2", "", 10, 0, new() { Id = 2 });

            //Assert
            Assert.Equal(0, list.Total);
        }

        /// <summary>
        /// Prueba la eliminación de una aplicación de un rol con error
        /// </summary>
        [Fact]
        public void DeleteApplicationWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.DeleteApplication(null, new() { Id = 1 }, new() { Id = 1 }));
        }
        #endregion
    }
}