using System.Data;

namespace Dal
{
    /// <summary>
    /// Clase base de la jerarquía de persistencias de entidades
    /// </summary>
    /// <typeparam name="T">Tipo de la entidad a persistir</typeparam>
    public abstract class PersistentBase
    {
        #region Attributes
        /// <summary>
        /// Conexión a la base de datos
        /// </summary>
        protected IDbConnection _connection;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la base de datos
        /// </summary>
        /// <param name="connection">Conexión a la base de datos</param>
        protected PersistentBase(IDbConnection connection)
        {
            _connection = connection;
        }
        #endregion

        #region Methods
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
        #endregion
    }
}
