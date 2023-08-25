using Dal.Exceptions;
using Dal.Log;
using System.Data;

namespace Dal
{
    /// <summary>
    /// Clase base de la jerarquía de persistencias de entidades y que registra un log de auditoría en las inserciones, actualizaciones o eliminaciones
    /// </summary>
    /// <typeparam name="T">Tipo de la entidad a persistir</typeparam>
    public abstract class PersistentBaseWithLog
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la base de datos
        /// </summary>
        /// <param name="connection">Conexión a la base de datos</param>
        protected PersistentBaseWithLog() { }
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
        /// <param name="connection">Conexión a la base de datos</param>
        private static void Log(string action, long id, string table, string sql, long user, IDbConnection connection)
        {
            try
            {
                PersistentLogDb persistentLogDb = new();
                _ = persistentLogDb.Insert(new() { Action = action, IdTable = id, Table = table, Sql = sql, User = (int)user }, connection);
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
        /// <param name="connection">Conexión a la base de datos</param>
        protected static void LogInsert(long id, string table, string sql, long user, IDbConnection connection)
        {
            Log("I", id, table, sql, user, connection);
        }

        /// <summary>
        /// Inserta un log de auditoría de una actualización en la base de datos
        /// </summary>
        /// <param name="id">Ientificador único en la tabla</param>
        /// <param name="table">Nombre de la tabla afectada</param>
        /// <param name="sql">SQL ejecutado</param>
        /// <param name="user">Ientificador del usuario que realiza la acción</param>
        /// <param name="connection">Conexión a la base de datos</param>
        protected static void LogUpdate(long id, string table, string sql, long user, IDbConnection connection)
        {
            Log("U", id, table, sql, user, connection);
        }

        /// <summary>
        /// Inserta un log de auditoría de una eliminación en la base de datos
        /// </summary>
        /// <param name="id">Ientificador único en la tabla</param>
        /// <param name="table">Nombre de la tabla afectada</param>
        /// <param name="sql">SQL ejecutado</param>
        /// <param name="user">Ientificador del usuario que realiza la acción</param>
        /// <param name="connection">Conexión a la base de datos</param>
        protected static void LogDelete(long id, string table, string sql, long user, IDbConnection connection)
        {
            Log("D", id, table, sql, user, connection);
        }
        #endregion
    }
}
