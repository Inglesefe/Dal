using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;

namespace Dal.Auth
{
    /// <summary>
    /// Métodos propios de la persistencia de roles
    /// </summary>
    public interface IPersistentRole : IPersistentWithLog<Role>
    {
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
        ListResult<User> ListUsers(string filters, string orders, int limit, int offset, Role role);

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
        ListResult<User> ListNotUsers(string filters, string orders, int limit, int offset, Role role);

        /// <summary>
        /// Asigna un usuario a un rol en la base de datos
        /// </summary>
        /// <param name="user">Usuario que se asigna al rol</param>
        /// <param name="role">Rol al que se le asigna el usuario</param>
        /// <param name="user1">Usuario que realiza la inserción</param>
        /// <returns>Usuario asignado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al asignar el usuario al rol</exception>
        User InsertUser(User user, Role role, User user1);

        /// <summary>
        /// Elimina un usuario de un rol de la base de datos
        /// </summary>
        /// <param name="user">Usuario a eliminarle al rol</param>
        /// <param name="role">Rol al que se le elimina el usuario</param>
        /// <param name="user1">Usuario que realiza la eliminación</param>
        /// <returns>Usuario eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el usuario del rol</exception>
        User DeleteUser(User user, Role role, User user1);

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
        ListResult<Application> ListApplications(string filters, string orders, int limit, int offset, Role role);

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
        ListResult<Application> ListNotApplications(string filters, string orders, int limit, int offset, Role role);

        /// <summary>
        /// Asigna una aplicación a un rol en la base de datos
        /// </summary>
        /// <param name="application">Aplicación que se asigna al rol</param>
        /// <param name="role">Rol al que se le asigna la aplicación</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Aplicación asignada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al asignar la aplicación al rol</exception>
        Application InsertApplication(Application application, Role role, User user);

        /// <summary>
        /// Elimina una aplicación de un rol de la base de datos
        /// </summary>
        /// <param name="application">Aplicación a eliminarle al rol</param>
        /// <param name="role">Rol al que se le elimina la aplicación</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Aplicación eliminada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar la aplicación del rol</exception>
        Application DeleteApplication(Application application, Role role, User user);
    }
}
