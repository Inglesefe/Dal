using Dal.Dto;
using Dal.Exceptions;
using Dal.Utils;
using Dapper;
using Entities.Auth;
using System.Data;
using static Dapper.SqlMapper;

namespace Dal.Auth
{
    /// <summary>
    /// Realiza la persistencia de los roles en la base de datos
    /// </summary>
    public class PersistentRole : PersistentBaseWithLog<Role>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la bse de datos
        /// </summary>
        /// <param name="connection">Conexión a la base de datos</param>
        public PersistentRole(IDbConnection connection) : base(connection) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de roles desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de roles</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los roles</exception>
        public override ListResult<Role> List(string filters, string orders, int limit, int offset)
        {
            ListResult<Role> result;
            try
            {
                QueryBuilder queryBuilder = new("idrole AS id, name", "role");
                OpenConnection();
                List<Role> roles = _connection.Query<Role>(queryBuilder.GetSelectForList(filters, orders, limit, offset)).ToList();
                int total = _connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                result = new(roles, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de usuarios", ex);
            }
            finally
            {
                CloseConnection();
            }
            return result;
        }

        /// <summary>
        /// Consulta un rol dado su identificador
        /// </summary>
        /// <param name="entity">Rol a consultar</param>
        /// <returns>Rol con los datos cargados desde la base de datos o null si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar el rol</exception>
        public override Role Read(Role entity)
        {
            try
            {
                OpenConnection();
                Role result = _connection.QuerySingleOrDefault<Role>("SELECT idrole AS id, name FROM role WHERE idrole = @Id", entity);
                if (result == null)
                {
                    entity = new();
                }
                else
                {
                    entity = result;
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el rol", ex);
            }
            finally
            {
                CloseConnection();
            }
            return entity;
        }

        /// <summary>
        /// Inserta un rol en la base de datos
        /// </summary>
        /// <param name="entity">Rol a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Rol insertado con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar el rol</exception>
        public override Role Insert(Role entity, User user)
        {
            try
            {
                OpenConnection();
                entity.Id = _connection.QuerySingleOrDefault<int>("INSERT INTO role (name) VALUES (@Name); SELECT LAST_INSERT_ID();", entity);
                LogInsert(entity.Id, "role", "INSERT INTO role (name) VALUES ('" + entity.Name + "')", user.Id);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar el rol", ex);
            }
            finally
            {
                CloseConnection();
            }
            return entity;
        }

        /// <summary>
        /// Actualiza un rol en la base de datos
        /// </summary>
        /// <param name="entity">Rol a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Rol actualizado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar el rol</exception>
        public override Role Update(Role entity, User user)
        {
            try
            {
                OpenConnection();
                _ = _connection.Execute("UPDATE role SET name = @Name WHERE idrole = @Id", entity);
                LogUpdate(entity.Id, "role", "UPDATE role SET name = '" + entity.Name + "' WHERE idrole = " + entity.Id, user.Id);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar el rol", ex);
            }
            finally
            {
                CloseConnection();
            }
            return entity;
        }

        /// <summary>
        /// Elimina un rol de la base de datos
        /// </summary>
        /// <param name="entity">Rol a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Rol eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el rol</exception>
        public override Role Delete(Role entity, User user)
        {
            try
            {
                OpenConnection();
                _ = _connection.Execute("DELETE FROM role WHERE idrole = @Id", entity);
                LogDelete(entity.Id, "role", "DELETE FROM role WHERE idrole = " + entity.Id, user.Id);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el rol", ex);
            }
            finally
            {
                CloseConnection();
            }
            return entity;
        }

        /// <summary>
        /// Trae un listado de usuarios asignados a un rol desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="role">Rol al que se le consultan los usuarios asignados</param>
        /// <returns>Listado de usuarios asignados al rol</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los usuarios</exception>
        public ListResult<User> ListUsers(string filters, string orders, int limit, int offset, Role role)
        {
            ListResult<User> result;
            try
            {
                QueryBuilder queryBuilder = new("u.iduser AS id, u.login as login, u.name as name, u.active as active", "user_role ur inner join user u on ur.iduser = u.iduser");
                OpenConnection();
                List<User> users = _connection.Query<User>(queryBuilder.GetSelectForList("ur.idrole = " + role.Id + (filters != "" ? " AND " : "") + filters, orders, limit, offset)).ToList();
                int total = _connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList("ur.idrole = " + role.Id + (filters != "" ? " AND " : "") + filters, orders));
                result = new(users, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de usuarios asignados al rol", ex);
            }
            finally
            {
                CloseConnection();
            }
            return result;
        }

        /// <summary>
        /// Trae un listado de usuarios no asignados a un rol desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="role">Rol al que se le consultan los usuarios no asignados</param>
        /// <returns>Listado de usuarios no asignados al rol</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los usuarios</exception>
        public ListResult<User> ListNotUsers(string filters, string orders, int limit, int offset, Role role)
        {
            ListResult<User> result;
            try
            {
                QueryBuilder queryBuilder = new("iduser AS id, login, name, active", "user");
                OpenConnection();
                List<User> users = _connection.Query<User>(queryBuilder.GetSelectForList("iduser NOT IN (SELECT iduser FROM user_role WHERE idrole = " + role.Id + ")" + (filters != "" ? " AND " : "") + filters, orders, limit, offset)).ToList();
                int total = _connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList("iduser NOT IN (SELECT iduser FROM user_role WHERE idrole = " + role.Id + ")" + (filters != "" ? " AND " : "") + filters, orders));
                result = new(users, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de usuarios no asignados al rol", ex);
            }
            finally
            {
                CloseConnection();
            }
            return result;
        }

        /// <summary>
        /// Asigna un usuario a un rol en la base de datos
        /// </summary>
        /// <param name="user">Usuario que se asigna al rol</param>
        /// <param name="role">Rol al que se le asigna el usuario</param>
        /// <param name="user1">Usuario que realiza la inserción</param>
        /// <returns>Usuario asignado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al asignar el usuario al rol</exception>
        public User InsertUser(User user, Role role, User user1)
        {
            try
            {
                OpenConnection();
                _ = _connection.Execute("INSERT INTO user_role (iduser, idrole) VALUES (@IdUser, @IdRole)", new { IdUser = user.Id, IdRole = role.Id });
                LogInsert(0, "user_role", "INSERT INTO user_role (iduser, idrole) VALUES (" + user.Id + ", " + role.Id + ")", user1.Id);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al asignar el rol al usuario", ex);
            }
            finally
            {
                CloseConnection();
            }
            return user;
        }

        /// <summary>
        /// Elimina un usuario de un rol de la base de datos
        /// </summary>
        /// <param name="user">Usuario a eliminarle al rol</param>
        /// <param name="role">Rol al que se le elimina el usuario</param>
        /// <param name="user1">Usuario que realiza la eliminación</param>
        /// <returns>Usuario eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el usuario del rol</exception>
        public User DeleteUser(User user, Role role, User user1)
        {
            try
            {
                OpenConnection();
                _ = _connection.Execute("DELETE FROM user_role WHERE iduser = @IdUser AND idrole = @IdRole", new { IdUser = user.Id, IdRole = role.Id });
                LogDelete(0, "user_role", "DELETE FROM user_role WHERE iduser = " + user.Id + " AND idrole = " + role.Id, user1.Id);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el rol del usuario", ex);
            }
            finally
            {
                CloseConnection();
            }
            return user;
        }

        /// <summary>
        /// Trae un listado de aplicaciones asignados a un rol desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="role">Rol al que se le consultan las aplicaciones asignadas</param>
        /// <returns>Listado de aplicaciones asignadas al rol</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar las aplicaciones</exception>
        public ListResult<Application> ListApplications(string filters, string orders, int limit, int offset, Role role)
        {
            ListResult<Application> result;
            try
            {
                QueryBuilder queryBuilder = new("a.idapplication AS id, a.name as name", "application_role ar inner join application a on ar.idapplication = a.idapplication");
                OpenConnection();
                List<Application> applications = _connection.Query<Application>(queryBuilder.GetSelectForList("ar.idrole = " + role.Id + (filters != "" ? " AND " : "") + filters, orders, limit, offset)).ToList();
                int total = _connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList("ar.idrole = " + role.Id + (filters != "" ? " AND " : "") + filters, orders));
                result = new(applications, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de aplicaciones asignados al rol", ex);
            }
            finally
            {
                CloseConnection();
            }
            return result;
        }

        /// <summary>
        /// Trae un listado de aplicaciones no asignados a un rol desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="role">Rol al que se le consultan las aplicaciones no asignadas</param>
        /// <returns>Listado de aplicaciones no asignadas al rol</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los usuarios</exception>
        public ListResult<Application> ListNotApplications(string filters, string orders, int limit, int offset, Role role)
        {
            ListResult<Application> result;
            try
            {
                QueryBuilder queryBuilder = new("idapplication AS id, name", "application");
                OpenConnection();
                List<Application> users = _connection.Query<Application>(queryBuilder.GetSelectForList("idapplication NOT IN (SELECT idapplication FROM application_role WHERE idrole = " + role.Id + ")" + (filters != "" ? " AND " : "") + filters, orders, limit, offset)).ToList();
                int total = _connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList("idapplication NOT IN (SELECT idapplication FROM application_role WHERE idrole = " + role.Id + ")" + (filters != "" ? " AND " : "") + filters, orders));
                result = new(users, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de aplicaciones no asignadas al rol", ex);
            }
            finally
            {
                CloseConnection();
            }
            return result;
        }

        /// <summary>
        /// Asigna una aplicación a un rol en la base de datos
        /// </summary>
        /// <param name="application">Aplicación que se asigna al rol</param>
        /// <param name="role">Rol al que se le asigna la aplicación</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Aplicación asignada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al asignar la aplicación al rol</exception>
        public Application InsertApplication(Application application, Role role, User user)
        {
            try
            {
                OpenConnection();
                _ = _connection.Execute("INSERT INTO application_role (idapplication, idrole) VALUES (@IdApplication, @IdRole)", new { IdApplication = application.Id, IdRole = role.Id });
                LogInsert(0, "application_role", "INSERT INTO application_role (idapplication, idrole) VALUES (" + application.Id + ", " + role.Id + ")", user.Id);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al asignar el rol a la aplicación", ex);
            }
            finally
            {
                CloseConnection();
            }
            return application;
        }

        /// <summary>
        /// Elimina una aplicación de un rol de la base de datos
        /// </summary>
        /// <param name="application">Aplicación a eliminarle al rol</param>
        /// <param name="role">Rol al que se le elimina la aplicación</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Aplicación eliminada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar la aplicación del rol</exception>
        public Application DeleteApplication(Application application, Role role, User user)
        {
            try
            {
                OpenConnection();
                _ = _connection.Execute("DELETE FROM application_role WHERE idapplication = @IdApplication AND idrole = @IdRole", new { IdApplication = application.Id, IdRole = role.Id });
                LogDelete(0, "application_role", "DELETE FROM application_role WHERE idapplication = " + application.Id + " AND idrole = " + role.Id, user.Id);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el rol de la aplicación", ex);
            }
            finally
            {
                CloseConnection();
            }
            return application;
        }
        #endregion
    }
}