using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;

namespace Dal.Auth
{
    /// <summary>
    /// Métodos propios de la persistencia de aplicaciones
    /// </summary>
    public interface IPersistentApplication : IPersistentWithLog<Application>
    {
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
        ListResult<Role> ListRoles(string filters, string orders, int limit, int offset, Application application);

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
        ListResult<Role> ListNotRoles(string filters, string orders, int limit, int offset, Application application);

        /// <summary>
        /// Asigna un rol a una aplicación en la base de datos
        /// </summary>
        /// <param name="role">Rol que se asigna a la aplicación</param>
        /// <param name="application">Aplicación al que se le asigna el rol</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Rol asignado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al asignar el rol a la aplicación</exception>
        Role InsertRole(Role role, Application application, User user);

        /// <summary>
        /// Elimina un rol de una aplicación de la base de datos
        /// </summary>
        /// <param name="role">Rol a eliminarle a la aplicación</param>
        /// <param name="application">Aplicación al que se le elimina el rol</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Rol eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el rol de la aplicación</exception>
        Role DeleteRole(Role role, Application application, User user);
    }
}
