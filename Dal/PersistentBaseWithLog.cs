using Dal.Dto;
using Dal.Exceptions;
using Dal.Log;
using Entities;
using Entities.Auth;

namespace Dal
{
    /// <summary>
    /// Clase base de la jerarquía de persistencias de entidades y que registra un log de auditoría en las inserciones, actualizaciones o eliminaciones
    /// </summary>
    /// <typeparam name="T">Tipo de la entidad a persistir</typeparam>
    public abstract class PersistentBaseWithLog<T> : IPersistentWithLog<T> where T : Entity
    {
        #region Attributes
        /// <summary>
        /// Conexión a la base de datos
        /// </summary>
        protected readonly string _connString;

        /// <summary>
        /// Persistencia en base de datos de los logs de base de datos
        /// </summary>
        protected readonly PersistentLogDb persistentLogDb;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la base de datos
        /// </summary>
        /// <param name="connString">Cadena de conexión a la base de datos</param>
        protected PersistentBaseWithLog(string connString)
        {
            _connString = connString;
            persistentLogDb = new PersistentLogDb(connString);
            Mapper.GetInstance();
        }
        #endregion

        #region Methods
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
                _ = persistentLogDb.Insert(new() { Action = action, IdTable = id, Table = table, Sql = sql, User = (int)user });
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

        /// <inheritdoc />
        public abstract ListResult<T> List(string filters, string orders, int limit, int offset);

        /// <inheritdoc />
        public abstract T Read(T entity);

        /// <inheritdoc />
        public abstract T Insert(T entity, User user);

        /// <inheritdoc />
        public abstract T Update(T entity, User user);

        /// <inheritdoc />
        public abstract T Delete(T entity, User user);
        #endregion
    }
}
