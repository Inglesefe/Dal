using Dal.Dto;
using Dal.Exceptions;
using Dal.Utils;
using Dapper;
using Entities.Auth;
using System.Data;

namespace Dal.Auth
{
    /// <summary>
    /// Realiza la persistencia de los usuarios en la base de datos
    /// </summary>
    public class PersistentUser : PersistentBaseWithLog, IPersistentUser
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la bse de datos
        /// </summary>
        /// <param name="connection">Conexión a la base de datos</param>
        public PersistentUser() : base() { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de usuarios desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Listado de usuarios</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los usuarios</exception>
        public ListResult<User> List(string filters, string orders, int limit, int offset, IDbConnection connection)
        {
            ListResult<User> result;
            try
            {
                QueryBuilder queryBuilder = new("iduser AS id, login, name, active", "user");
                using (connection)
                {
                    List<User> users = connection.Query<User>(queryBuilder.GetSelectForList(filters, orders, limit, offset)).ToList();
                    int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                    result = new(users, total);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de usuarios", ex);
            }
            return result;
        }

        /// <summary>
        /// Consulta un usuario dado su identificador
        /// </summary>
        /// <param name="entity">Usuario a consultar</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Usuario con los datos cargados desde la base de datos o null si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar el usuario</exception>
        public User Read(User entity, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    User result = connection.QuerySingleOrDefault<User>("SELECT iduser AS id, login, name, active FROM user WHERE iduser = @Id", entity);
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
                throw new PersistentException("Error al consultar el usuario", ex);
            }
            return entity;
        }

        /// <summary>
        /// Inserta un usuario en la base de datos
        /// </summary>
        /// <param name="entity">Usuario a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Usuario insertado con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar el usuario</exception>
        public User Insert(User entity, User user, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    entity.Id = connection.QuerySingleOrDefault<int>(
                        "INSERT INTO user (login, name, password, active) VALUES (@Login, @Name, SHA2(@login, 512), @Active); SELECT LAST_INSERT_ID();", entity);
                    LogInsert(
                        entity.Id,
                        "user",
                        "INSERT INTO user (login, name, password, active) VALUES ('" + entity.Login + "', '" + entity.Name + "', 'xxxxxx', " + (entity.Active ? "true" : "false") + ")",
                        user.Id, connection);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar el usuario", ex);
            }
            return entity;
        }

        /// <summary>
        /// Actualiza un usuario en la base de datos
        /// </summary>
        /// <param name="entity">Usuario a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Usuario actualizado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar el usuario</exception>
        public User Update(User entity, User user, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    _ = connection.Execute("UPDATE user SET login = @Login, name = @Name, active = @Active WHERE iduser = @Id", entity);
                    LogUpdate(
                        entity.Id,
                        "user",
                        "UPDATE user SET login = '" + entity.Login + "', name = '" + entity.Name + "', active = " + (entity.Active ? "true" : "false") + " WHERE iduser = " + entity.Id,
                        user.Id,
                        connection);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar el usuario", ex);
            }
            return entity;
        }

        /// <summary>
        /// Elimina un usuario de la base de datos
        /// </summary>
        /// <param name="entity">Usuario a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Usuario eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el usuario</exception>
        public User Delete(User entity, User user, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    _ = connection.Execute("DELETE FROM user WHERE iduser = @Id", entity);
                    LogDelete(entity.Id, "user", "DELETE FROM user WHERE iduser = " + entity.Id, user.Id, connection);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el usuario", ex);
            }
            return entity;
        }

        /// <summary>
        /// Consulta un usuario dado su login y contraseña
        /// </summary>
        /// <param name="entity">Usuario a consultar</param>
        /// <param name="password">Contraseña del usuario</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Usuario con los datos cargados desde la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar el usuario</exception>
        public User ReadByLoginAndPassword(User entity, string password, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    User result = connection.QuerySingleOrDefault<User>("SELECT iduser AS id, login, name, active FROM user WHERE login = @Login AND password = SHA2(@Pass, 512) AND active = 1", new { entity.Login, Pass = password });
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
                throw new PersistentException("Error al consultar el usuario", ex);
            }
            return entity;
        }

        /// <summary>
        /// Consulta un usuario existe dado su login y si su estado es activo
        /// </summary>
        /// <param name="entity">Usuario a consultar</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Usuario si existe y está activo en la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar el usuario</exception>
        public User ReadByLogin(User entity, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    User result = connection.QuerySingleOrDefault<User>("SELECT iduser AS id, login, name, active FROM user WHERE login = @Login AND active = 1", entity);
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
                throw new PersistentException("Error al consultar el usuario", ex);
            }
            return entity;
        }

        /// <summary>
        /// Actualiza la contraseña de un usuario en la base de datos
        /// </summary>
        /// <param name="entity">Usuario a actualizar</param>
        /// <param name="password">Nueva contraseña del usuario</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Usuario actualizado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar el usuario</exception>
        public User UpdatePassword(User entity, string password, User user, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    _ = connection.Execute("UPDATE user SET password = SHA2(@Pass, 512) WHERE iduser = @Id", new { Pass = password, entity.Id });
                    LogUpdate(entity.Id, "user", "UPDATE user SET password = 'xxxxxx' WHERE iduser = " + entity.Id, user.Id, connection);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar el usuario", ex);
            }
            return entity;
        }

        /// <summary>
        /// Trae un listado de roles asignados a un usuario desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="user">Usuario al que se le consultan los roles asignados</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Listado de roles asignados al usuario</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los roles</exception>
        public ListResult<Role> ListRoles(string filters, string orders, int limit, int offset, User user, IDbConnection connection)
        {
            ListResult<Role> result;
            try
            {
                QueryBuilder queryBuilder = new("r.idrole AS id, r.name as name", "user_role ur inner join role r on ur.idrole = r.idrole");
                using (connection)
                {
                    List<Role> roles = connection.Query<Role>(queryBuilder.GetSelectForList("ur.iduser = " + user.Id + (filters != "" ? " AND " : "") + filters, orders, limit, offset)).ToList();
                    int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList("ur.iduser = " + user.Id + (filters != "" ? " AND " : "") + filters, orders));
                    result = new(roles, total);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de roles asignados al usuario", ex);
            }
            return result;
        }

        /// <summary>
        /// Trae un listado de roles no asignados a un usuario desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="user">Usuario al que se le consultan los roles no asignados</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Listado de roles no asignados al usuario</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los roles</exception>
        public ListResult<Role> ListNotRoles(string filters, string orders, int limit, int offset, User user, IDbConnection connection)
        {
            ListResult<Role> result;
            try
            {
                QueryBuilder queryBuilder = new("idrole AS id, name", "role");
                using (connection)
                {
                    List<Role> roles = connection.Query<Role>(queryBuilder.GetSelectForList("idrole NOT IN (SELECT idrole FROM user_role WHERE iduser = " + user.Id + ")" + (filters != "" ? " AND " : "") + filters, orders, limit, offset)).ToList();
                    int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList("idrole NOT IN (SELECT idrole FROM user_role WHERE iduser = " + user.Id + ")" + (filters != "" ? " AND " : "") + filters, orders));
                    result = new(roles, total);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de roles no asignados al usuario", ex);
            }
            return result;
        }

        /// <summary>
        /// Asigna un rol a un usuario en la base de datos
        /// </summary>
        /// <param name="role">Rol que se asigna al usuario</param>
        /// <param name="user">Usuario al que se le asigna el rol</param>
        /// <param name="user1">Usuario que realiza la inserción</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Rol asignado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al asignar el rol al usuario</exception>
        public Role InsertRole(Role role, User user, User user1, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    _ = connection.Execute("INSERT INTO user_role (iduser, idrole) VALUES (@IdUser, @IdRole)", new { IdUser = user.Id, IdRole = role.Id });
                    LogInsert(0, "user_role", "INSERT INTO user_role (iduser, idrole) VALUES (" + user.Id + ", " + role.Id + ")", user1.Id, connection);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al asignar el rol al usuario", ex);
            }
            return role;
        }

        /// <summary>
        /// Elimina un rol de un usuario de la base de datos
        /// </summary>
        /// <param name="role">Rol a eliminarle al usuario</param>
        /// <param name="user">Usuario al que se le elimina el rol</param>
        /// <param name="user1">Usuario que realiza la eliminación</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Rol eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el rol del usuario</exception>
        public Role DeleteRole(Role role, User user, User user1, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    _ = connection.Execute("DELETE FROM user_role WHERE iduser = @IdUser AND idrole = @IdRole", new { IdUser = user.Id, IdRole = role.Id });
                    LogDelete(0, "user_role", "DELETE FROM user_role WHERE iduser = " + user.Id + " AND idrole = " + role.Id, user1.Id, connection);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el rol del usuario", ex);
            }
            return role;
        }
        #endregion
    }
}