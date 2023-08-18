using Dal.Dto;
using Dal.Exceptions;
using Dal.Noti;
using Entities.Noti;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

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
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .AddEnvironmentVariables()
                .Build();
            _persistent = new(new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de notificaciones con filtros, ordenamientos y límite
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void NotificationListTest()
        {
            ListResult<Notification> list = _persistent.List("idnotification = 1", "idnotification", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de notificaciones con filtros, ordenamientos y límite y con errores
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void NotificationListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _persistent.List("idnotificacion = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una notificación dado su identificador
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void NotificationReadTest()
        {
            Notification notification = new() { Id = 1 };
            notification = _persistent.Read(notification);

            Assert.Equal("leandrobaena@gmail.com", notification.To);
        }

        /// <summary>
        /// Prueba la consulta de una notificación que no existe dado su identificador
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void NotificationReadNotFoundTest()
        {
            Notification notification = new() { Id = 10 };
            notification = _persistent.Read(notification);

            Assert.Null(notification);
        }

        /// <summary>
        /// Prueba la inserción de una notificación
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void NotificationInsertTest()
        {
            Notification notification = new() { To = "leandrobaena@gmail.com", Subject = "Prueba de una notificación", Content = "<p>Prueba de notificación</p>", User = 1 };
            notification = _persistent.Insert(notification, new() { Id = 1 });

            Assert.NotEqual(0, notification.Id);
        }
        #endregion
    }
}
