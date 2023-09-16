using Dal.Dto;
using Dal.Exceptions;
using Dal.Log;
using Dapper;
using Entities;
using Entities.Admon;
using Entities.Auth;
using Entities.Config;
using Entities.Cont;
using Entities.Crm;
using Entities.Noti;

namespace Dal
{
    /// <summary>
    /// Clase base de la jerarquía de persistencias de entidades y que registra un log de auditoría en las inserciones, actualizaciones o eliminaciones
    /// </summary>
    /// <typeparam name="T">Tipo de la entidad a persistir</typeparam>
    public abstract class PersistentBaseWithLog<T> : IPersistentWithLog<T> where T : IEntity
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
            SqlMapper.SetTypeMap(typeof(AccountExecutive), new CustomPropertyTypeMap(typeof(AccountExecutive), (type, columnName) => new AccountExecutive().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Registration), new CustomPropertyTypeMap(typeof(Registration), (type, columnName) => new Registration().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Application), new CustomPropertyTypeMap(typeof(Application), (type, columnName) => new Application().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Role), new CustomPropertyTypeMap(typeof(Role), (type, columnName) => new Role().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(User), new CustomPropertyTypeMap(typeof(User), (type, columnName) => new User().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(City), new CustomPropertyTypeMap(typeof(City), (type, columnName) => new City().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Country), new CustomPropertyTypeMap(typeof(Country), (type, columnName) => new Country().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(IdentificationType), new CustomPropertyTypeMap(typeof(IdentificationType), (type, columnName) => new IdentificationType().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(IncomeType), new CustomPropertyTypeMap(typeof(IncomeType), (type, columnName) => new IncomeType().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Office), new CustomPropertyTypeMap(typeof(Office), (type, columnName) => new Office().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Parameter), new CustomPropertyTypeMap(typeof(Parameter), (type, columnName) => new Parameter().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Plan), new CustomPropertyTypeMap(typeof(Plan), (type, columnName) => new Plan().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Scale), new CustomPropertyTypeMap(typeof(Scale), (type, columnName) => new Scale().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(AccountNumber), new CustomPropertyTypeMap(typeof(AccountNumber), (type, columnName) => new AccountNumber().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(AccountType), new CustomPropertyTypeMap(typeof(AccountType), (type, columnName) => new AccountType().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(ConsecutiveNumber), new CustomPropertyTypeMap(typeof(ConsecutiveNumber), (type, columnName) => new ConsecutiveNumber().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(ConsecutiveType), new CustomPropertyTypeMap(typeof(ConsecutiveType), (type, columnName) => new ConsecutiveType().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Beneficiary), new CustomPropertyTypeMap(typeof(Beneficiary), (type, columnName) => new Beneficiary().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Owner), new CustomPropertyTypeMap(typeof(Owner), (type, columnName) => new Owner().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Notification), new CustomPropertyTypeMap(typeof(Notification), (type, columnName) => new Notification().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Template), new CustomPropertyTypeMap(typeof(Template), (type, columnName) => new Template().GetMapping(columnName)));
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
