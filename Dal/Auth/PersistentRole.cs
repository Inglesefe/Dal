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
    /// Realiza la persistencia de los roles en la base de datos
    /// </summary>
    public class PersistentRole : PersistentBaseWithLog<Role>, IPersistentRole
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la bse de datos
        /// </summary>
        /// <param name="connString">Cadena de conexión a la base de datos</param>
        public PersistentRole(string connString) : base(connString) { }
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
            try
            {
                QueryBuilder queryBuilder = new("idrole, name", "role");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<Role> roles = connection.Query<Role>(queryBuilder.GetSelectForList(filters, orders, limit, offset)).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                return new(roles, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de usuarios", ex);
            }
        }

        /// <summary>
        /// Consulta un rol dado su identificador
        /// </summary>
        /// <param name="entity">Rol a consultar</param>
        /// <returns>Rol con los datos cargados desde la base de datos o datos por defecto si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar el rol</exception>
        public override Role Read(Role entity)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                Role result = connection.QuerySingleOrDefault<Role>("SELECT idrole, name FROM role WHERE idrole = @Id", entity);
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
                throw new PersistentException("Error al consultar el rol", ex);
            }
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
                using IDbConnection connection = new MySqlConnection(_connString);
                entity.Id = connection.QuerySingleOrDefault<int>("INSERT INTO role (name) VALUES (@Name); SELECT LAST_INSERT_ID();", entity);
                LogInsert(entity.Id, "role", "INSERT INTO role (name) VALUES ('" + entity.Name + "')", user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar el rol", ex);
            }
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
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("UPDATE role SET name = @Name WHERE idrole = @Id", entity);
                LogUpdate(entity.Id, "role", "UPDATE role SET name = '" + entity.Name + "' WHERE idrole = " + entity.Id, user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar el rol", ex);
            }
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
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("DELETE FROM role WHERE idrole = @Id", entity);
                LogDelete(entity.Id, "role", "DELETE FROM role WHERE idrole = " + entity.Id, user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el rol", ex);
            }
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
            try
            {
                QueryBuilder queryBuilder = new("iduser, login, user, active", "v_user_role");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<User> users = connection.Query<User>(queryBuilder.GetSelectForList("idrole = " + role.Id + (filters != "" ? " AND " : "") + filters, orders, limit, offset)).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList("idrole = " + role.Id + (filters != "" ? " AND " : "") + filters, orders));
                return new(users, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de usuarios asignados al rol", ex);
            }
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
            try
            {
                QueryBuilder queryBuilder = new("iduser, login, name, active", "user");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<User> users = connection.Query<User>(
                    queryBuilder.GetSelectForList(
                        "iduser NOT IN (SELECT iduser FROM user_role WHERE idrole = " + role.Id + ")" + (filters != "" ? " AND " : "") + filters,
                        orders,
                        limit,
                        offset)).ToList();
                int total = connection.ExecuteScalar<int>(
                    queryBuilder.GetCountTotalSelectForList(
                        "iduser NOT IN (SELECT iduser FROM user_role WHERE idrole = " + role.Id + ")" + (filters != "" ? " AND " : "") + filters,
                        orders));
                return new(users, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de usuarios no asignados al rol", ex);
            }
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
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("INSERT INTO user_role (iduser, idrole) VALUES (@IdUser, @IdRole)", new { IdUser = user.Id, IdRole = role.Id });
                LogInsert(0, "user_role", "INSERT INTO user_role (iduser, idrole) VALUES (" + user.Id + ", " + role.Id + ")", user1.Id);
                return user;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al asignar el rol al usuario", ex);
            }
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
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("DELETE FROM user_role WHERE iduser = @IdUser AND idrole = @IdRole", new { IdUser = user.Id, IdRole = role.Id });
                LogDelete(0, "user_role", "DELETE FROM user_role WHERE iduser = " + user.Id + " AND idrole = " + role.Id, user1.Id);
                return user;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el rol del usuario", ex);
            }
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
            try
            {
                QueryBuilder queryBuilder = new("idapplication, application", "v_application_role");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<Application> applications = connection.Query<Application>(queryBuilder.GetSelectForList("idrole = " + role.Id + (filters != "" ? " AND " : "") + filters, orders, limit, offset)).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList("idrole = " + role.Id + (filters != "" ? " AND " : "") + filters, orders));
                return new(applications, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de aplicaciones asignados al rol", ex);
            }
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
            try
            {
                QueryBuilder queryBuilder = new("idapplication, name", "application");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<Application> users = connection.Query<Application>(
                    queryBuilder.GetSelectForList(
                        "idapplication NOT IN (SELECT idapplication FROM application_role WHERE idrole = " + role.Id + ")" + (filters != "" ? " AND " : "") + filters,
                        orders,
                        limit,
                        offset)).ToList();
                int total = connection.ExecuteScalar<int>(
                    queryBuilder.GetCountTotalSelectForList(
                        "idapplication NOT IN (SELECT idapplication FROM application_role WHERE idrole = " + role.Id + ")" + (filters != "" ? " AND " : "") + filters,
                        orders));
                return new(users, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de aplicaciones no asignadas al rol", ex);
            }
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
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("INSERT INTO application_role (idapplication, idrole) VALUES (@IdApplication, @IdRole)", new { IdApplication = application.Id, IdRole = role.Id });
                LogInsert(0, "application_role", "INSERT INTO application_role (idapplication, idrole) VALUES (" + application.Id + ", " + role.Id + ")", user.Id);
                return application;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al asignar el rol a la aplicación", ex);
            }
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
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("DELETE FROM application_role WHERE idapplication = @IdApplication AND idrole = @IdRole", new { IdApplication = application.Id, IdRole = role.Id });
                LogDelete(0, "application_role", "DELETE FROM application_role WHERE idapplication = " + application.Id + " AND idrole = " + role.Id, user.Id);
                return application;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el rol de la aplicación", ex);
            }
        }
        #endregion
    }
}