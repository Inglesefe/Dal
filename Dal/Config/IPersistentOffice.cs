using Dal.Dto;
using Dal.Exceptions;
using Entities.Admon;
using Entities.Auth;
using Entities.Config;

namespace Dal.Config
{
    /// <summary>
    /// Métodos propios de la persistencia de oficinas
    /// </summary>
    public interface IPersistentOffice : IPersistentWithLog<Office>
    {
        #region Methods
        /// <summary>
        /// Trae un listado de ejecutivos de cuenta asignados a una oficina desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="office">Oficina a la que se le consultan los ejecutivos de cuenta asignados</param>
        /// <returns>Listado de ejecutivos de cuenta asignados a la oficina</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los ejecutivos de cuenta</exception>
        ListResult<AccountExecutive> ListAccountExecutives(string filters, string orders, int limit, int offset, Office office);

        /// <summary>
        /// Trae un listado de ejecutivos de cuenta no asignados a una oficina desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="office">Oficina a la que se le consultan los ejecutivos de cuenta no asignados</param>
        /// <returns>Listado de ejecutivos de cuenta no asignados a la oficina</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los ejecutivos de cuenta</exception>
        ListResult<AccountExecutive> ListNotAccountExecutives(string filters, string orders, int limit, int offset, Office office);

        /// <summary>
        /// Asigna un ejecutivo de cuenta a una oficina en la base de datos
        /// </summary>
        /// <param name="executive">Ejecutivo de cuenta que se asigna a la oficina</param>
        /// <param name="office">Oficina a la que se le asigna el ejecutivo de cuenta</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Ejecutivo de cuenta asignado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al asignar el ejecutivo de cuenta a la oficina</exception>
        AccountExecutive InsertAccountExecutive(AccountExecutive executive, Office office, User user);

        /// <summary>
        /// Elimina un ejecutivo de cuenta de una oficina de la base de datos
        /// </summary>
        /// <param name="executive">Ejecutivo de cuenta a eliminarle a la oficina</param>
        /// <param name="office">Oficina a la que se le elimina el ejecutivo de cuenta</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Ejecutivo de cuenta eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el ejecutivo de cuenta de la oficina</exception>
        AccountExecutive DeleteAccountExecutive(AccountExecutive executive, Office office, User user);
        #endregion
    }
}
