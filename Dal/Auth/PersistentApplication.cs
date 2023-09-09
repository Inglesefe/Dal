using Dal.Dto;
using Dal.Exceptions;
using Dal.Utils;
using Dapper;
using Entities.Auth;
using MySql.Data.MySqlClient;
using System.Data;

namespace Dal.Auth
{
    /// <summary>
    /// Realiza la persistencia de las aplicaciones en la base de datos
    /// </summary>
    public class PersistentApplication : PersistentBaseWithLog<Application>, IPersistentApplication
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la bse de datos
        /// </summary>
        /// <param name="connString">Cadena de conexión a la base de datos</param>
        public PersistentApplication(string connString) : base(connString) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de aplicaciones desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de aplicaciones</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar las aplicaciones</exception>
        public override ListResult<Application> List(string filters, string orders, int limit, int offset)
        {
            try
            {
                QueryBuilder queryBuilder = new("idapplication, name", "application");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<Application> applications = connection.Query<Application>(queryBuilder.GetSelectForList(filters, orders, limit, offset)).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                return new(applications, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de aplicaciones", ex);
            }
        }

        /// <summary>
        /// Consulta una aplicación dado su identificador
        /// </summary>
        /// <param name="entity">Aplicación a consultar</param>
        /// <returns>Aplicación con los datos cargados desde la base de datos o datos por defecto si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar la aplicación</exception>
        public override Application Read(Application entity)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                Application result = connection.QuerySingleOrDefault<Application>("SELECT idapplication, name FROM application WHERE idapplication = @Id", entity);
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
                throw new PersistentException("Error al consultar la aplicación", ex);
            }
        }

        /// <summary>
        /// Inserta una aplicación en la base de datos
        /// </summary>
        /// <param name="entity">Aplicación a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Aplicación insertada con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar la aplicación</exception>
        public override Application Insert(Application entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                entity.Id = connection.QuerySingle<int>("INSERT INTO application (name) VALUES (@Name); SELECT LAST_INSERT_ID();", entity);
                LogInsert(entity.Id, "application", "INSERT INTO application (name) VALUES ('" + entity.Name + "')", user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar la aplicación", ex);
            }
        }

        /// <summary>
        /// Actualiza una aplicación en la base de datos
        /// </summary>
        /// <param name="entity">Aplicación a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Aplicación actualizada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar la aplicación</exception>
        public override Application Update(Application entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("UPDATE application SET name = @Name WHERE idapplication = @Id", entity);
                LogUpdate(entity.Id, "application", "UPDATE application SET name = '" + entity.Name + "' WHERE idapplication = " + entity.Id, user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar la aplicación", ex);
            }
        }

        /// <summary>
        /// Elimina una aplicación de la base de datos
        /// </summary>
        /// <param name="entity">Aplicación a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Aplicación eliminada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar la aplicación</exception>
        public override Application Delete(Application entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("DELETE FROM application WHERE idapplication = @Id", entity);
                LogDelete(entity.Id, "application", "DELETE FROM application WHERE idapplication = " + entity.Id, user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar la aplicación", ex);
            }
        }

        /// <summary>
        /// Trae un listado de roles asignados a una aplicación desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="application">Aplicación al que se le consultan los roles asignados</param>
        /// <returns>Listado de roles asignados a la aplicación</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los roles</exception>
        public ListResult<Role> ListRoles(string filters, string orders, int limit, int offset, Application application)
        {
            try
            {
                QueryBuilder queryBuilder = new("idrole, role", "v_application_role");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<Role> roles = connection.Query<Role>(queryBuilder.GetSelectForList("idapplication = " + application.Id + (filters != "" ? " AND " : "") + filters, orders, limit, offset)).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList("idapplication = " + application.Id + (filters != "" ? " AND " : "") + filters, orders));
                return new(roles, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de roles asignados a la aplicación", ex);
            }
        }

        /// <summary>
        /// Trae un listado de roles no asignados a una aplicación desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="application">Aplicación a la que se le consultan los roles no asignados</param>
        /// <returns>Listado de roles no asignados a la aplicación</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los roles</exception>
        public ListResult<Role> ListNotRoles(string filters, string orders, int limit, int offset, Application application)
        {
            try
            {
                QueryBuilder queryBuilder = new("idrole, name", "role");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<Role> roles = connection.Query<Role>(
                        queryBuilder.GetSelectForList(
                            "idrole NOT IN (SELECT idrole FROM application_role WHERE idapplication = " + application.Id + ")" + (filters != "" ? " AND " : "") + filters,
                            orders,
                            limit,
                            offset)).ToList();
                int total = connection.ExecuteScalar<int>(
                    queryBuilder.GetCountTotalSelectForList(
                        "idrole NOT IN (SELECT idrole FROM application_role WHERE idapplication = " + application.Id + ")" + (filters != "" ? " AND " : "") + filters,
                        orders));
                return new(roles, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de roles no asignados a la aplicación", ex);
            }
        }

        /// <summary>
        /// Asigna un rol a una aplicación en la base de datos
        /// </summary>
        /// <param name="role">Rol que se asigna a la aplicación</param>
        /// <param name="application">Aplicación al que se le asigna el rol</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Rol asignado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al asignar el rol a la aplicación</exception>
        public Role InsertRole(Role role, Application application, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("INSERT INTO application_role (idapplication, idrole) VALUES (@IdApplication, @IdRole)", new { IdApplication = application.Id, IdRole = role.Id });
                LogInsert(0, "application_role", "INSERT INTO application_role (idapplication, idrole) VALUES (" + application.Id + ", " + role.Id + ")", user.Id);
                return role;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al asignar el rol a la aplicación", ex);
            }
        }

        /// <summary>
        /// Elimina un rol de una aplicación de la base de datos
        /// </summary>
        /// <param name="role">Rol a eliminarle a la aplicación</param>
        /// <param name="application">Aplicación al que se le elimina el rol</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Rol eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el rol de la aplicación</exception>
        public Role DeleteRole(Role role, Application application, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("DELETE FROM application_role WHERE idapplication = @IdApplication AND idrole = @IdRole", new { IdApplication = application.Id, IdRole = role.Id });
                LogDelete(0, "application_role", "DELETE FROM application_role WHERE idapplication = " + application.Id + " AND idrole = " + role.Id, user.Id);
                return role;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el rol de la aplicación", ex);
            }
        }
        #endregion
    }
}