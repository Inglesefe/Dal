using Dal.Dto;
using Dal.Exceptions;
using Dal.Noti;
using Entities.Noti;
using Microsoft.Extensions.Configuration;

namespace Dal.Test.Noti
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de notificaciones
    /// </summary>
    [Collection("Tests")]
    public class NotificationTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de persistencia en la base de datos
        /// </summary>
        private readonly PersistentNotification _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public NotificationTest()
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
        /// Prueba la consulta de un listado de notificaciones con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<Notification> list = _persistent.List("idnotification = 1", "idnotification", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de notificaciones con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.List("idnotificacion = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una notificación dado su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            Notification notification = new() { Id = 1 };

            //Act
            notification = _persistent.Read(notification);

            //Assert
            Assert.Equal("leandrobaena@gmail.com", notification.To);
        }

        /// <summary>
        /// Prueba la consulta de una notificación que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            Notification notification = new() { Id = 10 };

            //Act
            notification = _persistent.Read(notification);

            //Assert
            Assert.Equal(0, notification.Id);
        }

        /// <summary>
        /// Prueba la consulta de una notificación con error
        /// </summary>
        [Fact]
        public void ReadWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Read(null));
        }

        /// <summary>
        /// Prueba la inserción de una notificación
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            Notification notification = new() { To = "leandrobaena@gmail.com", Subject = "Prueba de una notificación", Content = "<p>Prueba de notificación</p>", User = 1 };

            //Act
            notification = _persistent.Insert(notification, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, notification.Id);
        }

        /// <summary>
        /// Prueba la inserción de una notificación con error
        /// </summary>
        [Fact]
        public void InsertWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Insert(null, new() { Id = 1 }));
        }
        #endregion
    }
}
