using Dal.Dto;
using Dal.Exceptions;
using Dal.Utils;
using Dapper;
using Entities.Auth;
using Entities.Noti;
using MySql.Data.MySqlClient;
using System.Data;

namespace Dal.Noti
{
    /// <summary>
    /// Realiza la persistencia de las notificaciones en la base de datos
    /// </summary>
    public class PersistentNotification : PersistentBaseWithLog<Notification>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la base de datos
        /// <param name="connString">Cadena de conexión a la base de datos</param>
        /// </summary>
        public PersistentNotification(string connString) : base(connString) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de notificaciones desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de notificaciones</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar las notificaciones</exception>
        public override ListResult<Notification> List(string filters, string orders, int limit, int offset)
        {
            try
            {
                QueryBuilder queryBuilder = new("idnotification, date, `to`, subject, content, iduser", "notification");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<Notification> notifications = connection.Query<Notification>(queryBuilder.GetSelectForList(filters, orders, limit, offset)).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                return new(notifications, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de notificaciones", ex);
            }
        }

        /// <summary>
        /// Consulta una notificación dada su identificador
        /// </summary>
        /// <param name="entity">Notificación a consultar</param>
        /// <returns>Notificación con los datos cargados desde la base de datos o datos por defecto si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar la notificación</exception>
        public override Notification Read(Notification entity)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                Notification result = connection.QuerySingleOrDefault<Notification>(
                    "SELECT idnotification, date, `to`, subject, content, iduser FROM notification WHERE idnotification = @Id", entity);
                if (result == null)
                {
                    entity = new();
                }
                else
                {
                    entity = result;
                }
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar la notificación", ex);
            }
        }

        /// <summary>
        /// Inserta una notificación en la base de datos
        /// </summary>
        /// <param name="entity">Notificación a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Notificación insertada con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar la notificación</exception>
        public override Notification Insert(Notification entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                entity.Id = connection.QuerySingle<int>(
                    "INSERT INTO notification (date, `to`, subject, content, iduser) VALUES (NOW(), @To, @Subject, @Content, @User); SELECT LAST_INSERT_ID();", entity);
                LogInsert(entity.Id, "template", "INSERT INTO notification (subject, `to`, idtemplate, content) VALUES ('" + entity.Subject + "', '" + entity.Content + "')", user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar la notificación", ex);
            }
        }

        /// <summary>
        /// Actualiza una notificación en la base de datos
        /// </summary>
        /// <param name="entity">Notificaión a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Notificación actualizada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar la notificación</exception>
        public override Notification Update(Notification entity, User user)
        {
            //Una notificación no se modifica una vez enviada
            return entity;
        }

        /// <summary>
        /// Elimina una notificación de la base de datos
        /// </summary>
        /// <param name="entity">Notificación a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Notificación eliminada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar la plantilla</exception>
        public override Notification Delete(Notification entity, User user)
        {
            //Una notificación no se elimina una vez enviada
            return entity;
        }
        #endregion
    }
}
