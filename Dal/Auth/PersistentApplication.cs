using Dal.Dto;
using Dal.Exceptions;
using Dal.Utils;
using Dapper;
using Entities.Auth;
using System.Data;

namespace Dal.Auth
{
    /// <summary>
    /// Realiza la persistencia de las aplicaciones en la base de datos
    /// </summary>
    public class PersistentApplication : PersistentBaseWithLog, IPersistentApplication
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la bse de datos
        /// </summary>
        /// <param name="connection">Conexión a la base de datos</param>
        public PersistentApplication() : base() { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de aplicaciones desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Listado de aplicaciones</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar las aplicaciones</exception>
        public ListResult<Application> List(string filters, string orders, int limit, int offset, IDbConnection connection)
        {
            ListResult<Application> result;
            try
            {
                QueryBuilder queryBuilder = new("idapplication AS id, name", "application");
                using (connection)
                {
                    List<Application> applications = connection.Query<Application>(queryBuilder.GetSelectForList(filters, orders, limit, offset)).ToList();
                    int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                    result = new(applications, total);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de aplicaciones", ex);
            }
            return result;
        }

        /// <summary>
        /// Consulta una aplicación dado su identificador
        /// </summary>
        /// <param name="entity">Aplicación a consultar</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Aplicación con los datos cargados desde la base de datos o null si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar la aplicación</exception>
        public Application Read(Application entity, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    Application result = connection.QuerySingleOrDefault<Application>("SELECT idapplication AS id, name FROM application WHERE idapplication = @Id", entity);
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
                throw new PersistentException("Error al consultar la aplicación", ex);
            }
            return entity;
        }

        /// <summary>
        /// Inserta una aplicación en la base de datos
        /// </summary>
        /// <param name="entity">Aplicación a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Aplicación insertada con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar la aplicación</exception>
        public Application Insert(Application entity, User user, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    entity.Id = connection.QuerySingle<int>("INSERT INTO application (name) VALUES (@Name); SELECT LAST_INSERT_ID();", entity);
                    LogInsert(entity.Id, "application", "INSERT INTO application (name) VALUES ('" + entity.Name + "')", user.Id, connection);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar la aplicación", ex);
            }
            return entity;
        }

        /// <summary>
        /// Actualiza una aplicación en la base de datos
        /// </summary>
        /// <param name="entity">Aplicación a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Aplicación actualizada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar la aplicación</exception>
        public Application Update(Application entity, User user, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    _ = connection.Execute("UPDATE application SET name = @Name WHERE idapplication = @Id", entity);
                    LogUpdate(entity.Id, "application", "UPDATE application SET name = '" + entity.Name + "' WHERE idapplication = " + entity.Id, user.Id, connection);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar la aplicación", ex);
            }
            return entity;
        }

        /// <summary>
        /// Elimina una aplicación de la base de datos
        /// </summary>
        /// <param name="entity">Aplicación a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Aplicación eliminada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar la aplicación</exception>
        public Application Delete(Application entity, User user, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    _ = connection.Execute("DELETE FROM application WHERE idapplication = @Id", entity);
                    LogDelete(entity.Id, "application", "DELETE FROM application WHERE idapplication = " + entity.Id, user.Id, connection);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar la aplicación", ex);
            }
            return entity;
        }

        /// <summary>
        /// Trae un listado de roles asignados a una aplicación desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="application">Aplicación al que se le consultan los roles asignados</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Listado de roles asignados a la aplicación</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los roles</exception>
        public ListResult<Role> ListRoles(string filters, string orders, int limit, int offset, Application application, IDbConnection connection)
        {
            ListResult<Role> result;
            try
            {
                QueryBuilder queryBuilder = new("r.idrole AS id, r.name as name", "application_role ar inner join role r on ar.idrole = r.idrole");
                using (connection)
                {
                    List<Role> roles = connection.Query<Role>(queryBuilder.GetSelectForList("ar.idapplication = " + application.Id + (filters != "" ? " AND " : "") + filters, orders, limit, offset)).ToList();
                    int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList("ar.idapplication = " + application.Id + (filters != "" ? " AND " : "") + filters, orders));
                    result = new(roles, total);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de roles asignados a la aplicación", ex);
            }
            return result;
        }

        /// <summary>
        /// Trae un listado de roles no asignados a una aplicación desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="application">Aplicación a la que se le consultan los roles no asignados</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Listado de roles no asignados a la aplicación</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los roles</exception>
        public ListResult<Role> ListNotRoles(string filters, string orders, int limit, int offset, Application application, IDbConnection connection)
        {
            ListResult<Role> result;
            try
            {
                QueryBuilder queryBuilder = new("idrole AS id, name", "role");
                using (connection)
                {
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
                    result = new(roles, total);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de roles no asignados a la aplicación", ex);
            }
            return result;
        }

        /// <summary>
        /// Asigna un rol a una aplicación en la base de datos
        /// </summary>
        /// <param name="role">Rol que se asigna a la aplicación</param>
        /// <param name="application">Aplicación al que se le asigna el rol</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Rol asignado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al asignar el rol a la aplicación</exception>
        public Role InsertRole(Role role, Application application, User user, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    _ = connection.Execute("INSERT INTO application_role (idapplication, idrole) VALUES (@IdApplication, @IdRole)", new { IdApplication = application.Id, IdRole = role.Id });
                    LogInsert(0, "application_role", "INSERT INTO application_role (idapplication, idrole) VALUES (" + application.Id + ", " + role.Id + ")", user.Id, connection);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al asignar el rol a la aplicación", ex);
            }
            return role;
        }

        /// <summary>
        /// Elimina un rol de una aplicación de la base de datos
        /// </summary>
        /// <param name="role">Rol a eliminarle a la aplicación</param>
        /// <param name="application">Aplicación al que se le elimina el rol</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Rol eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el rol de la aplicación</exception>
        public Role DeleteRole(Role role, Application application, User user, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    _ = connection.Execute("DELETE FROM application_role WHERE idapplication = @IdApplication AND idrole = @IdRole", new { IdApplication = application.Id, IdRole = role.Id });
                    LogDelete(0, "application_role", "DELETE FROM application_role WHERE idapplication = " + application.Id + " AND idrole = " + role.Id, user.Id, connection);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el rol de la aplicación", ex);
            }
            return role;
        }
        #endregion
    }
}