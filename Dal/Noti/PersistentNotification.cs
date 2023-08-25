﻿using Dal.Dto;
using Dal.Exceptions;
using Dal.Utils;
using Dapper;
using Entities.Auth;
using Entities.Noti;
using System.Data;

namespace Dal.Noti
{
    /// <summary>
    /// Realiza la persistencia de las notificaciones en la base de datos
    /// </summary>
    public class PersistentNotification : PersistentBaseWithLog, IPersistentWithLog<Notification>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la base de datos
        /// </summary>
        public PersistentNotification() : base() { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de notificaciones desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Listado de notificaciones</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar las notificaciones</exception>
        public ListResult<Notification> List(string filters, string orders, int limit, int offset, IDbConnection connection)
        {
            ListResult<Notification> result;
            try
            {
                QueryBuilder queryBuilder = new("idnotification AS id, date, `to`, subject, content, iduser as user", "notification");
                using (connection)
                {
                    List<Notification> notifications = connection.Query<Notification>(queryBuilder.GetSelectForList(filters, orders, limit, offset)).ToList();
                    int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                    result = new(notifications, total);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de notificaciones", ex);
            }
            return result;
        }

        /// <summary>
        /// Consulta una notificación dada su identificador
        /// </summary>
        /// <param name="entity">Notificación a consultar</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Notificación con los datos cargados desde la base de datos o null si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar la notificación</exception>
        public Notification Read(Notification entity, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    Notification result = connection.QuerySingleOrDefault<Notification>("SELECT idnotification AS id, date, `to`, subject, content, iduser as user FROM notification WHERE idnotification = @Id", entity);
                    if (result == null)
                    {
                        entity = new();
                    }
                    else
                    {
                        entity = result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar la notificación", ex);
            }
            return entity;
        }

        /// <summary>
        /// Inserta una notificación en la base de datos
        /// </summary>
        /// <param name="entity">Notificación a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Notificación insertada con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar la notificación</exception>
        public Notification Insert(Notification entity, User user, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    entity.Id = connection.QuerySingle<int>("INSERT INTO notification (date, `to`, subject, content, iduser) VALUES (NOW(), @To, @Subject, @Content, @User); SELECT LAST_INSERT_ID();", entity);
                    LogInsert(entity.Id, "template", "INSERT INTO notification (subject, `to`, idtemplate, content) VALUES ('" + entity.Subject + "', '" + entity.Content + "')", user.Id, connection);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar la notificación", ex);
            }
            return entity;
        }

        /// <summary>
        /// Actualiza una notificación en la base de datos
        /// </summary>
        /// <param name="entity">Notificaión a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Notificación actualizada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar la notificación</exception>
        public Notification Update(Notification entity, User user, IDbConnection connection)
        {
            //Una notificación no se modifica una vez enviada
            return entity;
        }

        /// <summary>
        /// Elimina una notificación de la base de datos
        /// </summary>
        /// <param name="entity">Notificación a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Notificación eliminada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar la plantilla</exception>
        public Notification Delete(Notification entity, User user, IDbConnection connection)
        {
            //Una notificación no se elimina una vez enviada
            return entity;
        }
        #endregion
    }
}
