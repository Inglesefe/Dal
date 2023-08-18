using Dal.Dto;
using Dal.Exceptions;
using Dal.Log;
using Entities;
using Entities.Auth;
using System.Data;

namespace Dal
{
    /// <summary>
    /// Clase base de la jerarquía de persistencias de entidades y que registra un log de auditoría en las inserciones, actualizaciones o eliminaciones
    /// </summary>
    /// <typeparam name="T">Tipo de la entidad a persistir</typeparam>
    public abstract class PersistentBaseWithLog<T> where T : EntityBase
    {
        #region Attributes
        /// <summary>
        /// Conexión a la base de datos
        /// </summary>
        protected IDbConnection _connection;

        /// <summary>
        /// Persistencia de los registros de auditoría de base de datos
        /// </summary>
        protected PersistentLogDb _persistentLogDb;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la base de datos
        /// </summary>
        /// <param name="connection">Conexión a la base de datos</param>
        protected PersistentBaseWithLog(IDbConnection connection)
        {
            _connection = connection;
            _persistentLogDb = new PersistentLogDb(connection);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de registros aplicando filtros, ordenamientos y límites de registros a traer
        /// </summary>
        /// <param name="filters">Filtros a aplicar en la consulta</param>
        /// <param name="orders">Ordenamientos a aplicar en la consulta</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Registro inicial desde el que se cuenta el límite de registros</param>
        /// <returns>Listado de registros y el total que traería sin aplicar límites</returns>
        public abstract ListResult<T> List(string filters, string orders, int limit, int offset);

        /// <summary>
        /// Trae una entidad con los datos cargados desde la base de datos
        /// </summary>
        /// <param name="entity">Entidad a cargar dado su identificador en la base de datos</param>
        /// <returns>Entidad con los datos cargados desde la base de datos</returns>
        public abstract T Read(T entity);

        /// <summary>
        /// Inserta una entidad en la base de datos
        /// </summary>
        /// <param name="entity">Entidad a insertar en la base de datos</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Entidad insertada en la base de datos, con el id generado en ella si aplica</returns>
        public abstract T Insert(T entity, User user);

        /// <summary>
        /// Actualiza una entidad en la base de datos
        /// </summary>
        /// <param name="entity">Entidad a actualizar en la base de datos</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Entidad actualizada en la base de datos</returns>
        public abstract T Update(T entity, User user);

        /// <summary>
        /// Elimina una entidad en la base de datos
        /// </summary>
        /// <param name="entity">Entidad a eliminar en la base de datos</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Entidad eliminada en la base de datos</returns>
        public abstract T Delete(T entity, User user);

        /// <summary>
        /// Verifica que si la conexión a la base de datos no está abierta se abra
        /// </summary>
        protected void OpenConnection()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        /// <summary>
        /// Verifica que si la conexión a la base de datos no está cerrada se cierre
        /// </summary>
        protected void CloseConnection()
        {
            if (_connection.State != ConnectionState.Closed)
            {
                _connection.Close();
            }
        }

        /// <summary>
        /// Inserta un log de auditoría de base de datos
        /// </summary>
        /// <param name="action">Acción ejecutada I - Insertar, U - Actualizar, D - Eliminar</param>
        /// <param name="id">Ientificador único en la tabla</param>
        /// <param name="table">Nombre de la tabla afectada</param>
        /// <param name="sql">SQL ejecutado</param>
        /// <param name="user">Ientificador del usuario que realiza la acción</param>
        private void Log(string action, long id, string table, string sql, long user)
        {
            try
            {
                _ = _persistentLogDb.Insert(new() { Action = action, IdTable = id, Table = table, Sql = sql, User = (int)user });
            }
            catch (PersistentException)
            {
                //No hacer nada
            }
        }

        /// <summary>
        /// Inserta un log de auditoría de una inserción en la base de datos
        /// </summary>
        /// <param name="id">Ientificador único en la tabla</param>
        /// <param name="table">Nombre de la tabla afectada</param>
        /// <param name="sql">SQL ejecutado</param>
        /// <param name="user">Ientificador del usuario que realiza la acción</param>
        protected void LogInsert(long id, string table, string sql, long user)
        {
            Log("I", id, table, sql, user);
        }

        /// <summary>
        /// Inserta un log de auditoría de una actualización en la base de datos
        /// </summary>
        /// <param name="id">Ientificador único en la tabla</param>
        /// <param name="table">Nombre de la tabla afectada</param>
        /// <param name="sql">SQL ejecutado</param>
        /// <param name="user">Ientificador del usuario que realiza la acción</param>
        protected void LogUpdate(long id, string table, string sql, long user)
        {
            Log("U", id, table, sql, user);
        }

        /// <summary>
        /// Inserta un log de auditoría de una eliminación en la base de datos
        /// </summary>
        /// <param name="id">Ientificador único en la tabla</param>
        /// <param name="table">Nombre de la tabla afectada</param>
        /// <param name="sql">SQL ejecutado</param>
        /// <param name="user">Ientificador del usuario que realiza la acción</param>
        protected void LogDelete(long id, string table, string sql, long user)
        {
            Log("D", id, table, sql, user);
        }
        #endregion
    }
}
