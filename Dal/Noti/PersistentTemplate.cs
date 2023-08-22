using Dal.Dto;
using Dal.Exceptions;
using Dal.Utils;
using Dapper;
using Entities.Auth;
using Entities.Noti;
using System.Data;

namespace Dal.Noti
{
    /// <summary>
    /// Realiza la persistencia de las plantillas en la base de datos
    /// </summary>
    public class PersistentTemplate : PersistentBaseWithLog, IPersistentWithLog<Template>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la base de datos
        /// </summary>
        /// <param name="connection">Conexión a la base de datos</param>
        public PersistentTemplate(IDbConnection connection) : base(connection) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de plantillas desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de plantillas</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar las plantillas</exception>
        public ListResult<Template> List(string filters, string orders, int limit, int offset)
        {
            ListResult<Template> result;
            try
            {
                QueryBuilder queryBuilder = new("idtemplate AS id, name, content", "template");
                OpenConnection();
                List<Template> templates = _connection.Query<Template>(queryBuilder.GetSelectForList(filters, orders, limit, offset)).ToList();
                int total = _connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                result = new(templates, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de plantillas", ex);
            }
            finally
            {
                CloseConnection();
            }
            return result;
        }

        /// <summary>
        /// Consulta una plantilla dado su identificador
        /// </summary>
        /// <param name="entity">Plantilla a consultar</param>
        /// <returns>Plnatilla con los datos cargados desde la base de datos o null si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar la plantilla</exception>
        public Template Read(Template entity)
        {
            try
            {
                OpenConnection();
                Template result = _connection.QuerySingleOrDefault<Template>("SELECT idtemplate AS id, name, content FROM template WHERE idtemplate = @Id", entity);
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
                throw new PersistentException("Error al consultar la plantilla", ex);
            }
            finally
            {
                CloseConnection();
            }
            return entity;
        }

        /// <summary>
        /// Inserta una plantilla en la base de datos
        /// </summary>
        /// <param name="entity">Plantilla a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Plantilla insertada con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar la plantilla</exception>
        public Template Insert(Template entity, User user)
        {
            try
            {
                OpenConnection();
                entity.Id = _connection.QuerySingle<int>("INSERT INTO template (name, content) VALUES (@Name, @Content); SELECT LAST_INSERT_ID();", entity);
                LogInsert(entity.Id, "template", "INSERT INTO template (name, content) VALUES ('" + entity.Name + "', '" + entity.Content + "')", user.Id);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar la plantilla", ex);
            }
            finally
            {
                CloseConnection();
            }
            return entity;
        }

        /// <summary>
        /// Actualiza una plantilla en la base de datos
        /// </summary>
        /// <param name="entity">Plantilla a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Plantilla actualizada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar la plantilla</exception>
        public Template Update(Template entity, User user)
        {
            try
            {
                OpenConnection();
                _ = _connection.Execute("UPDATE template SET name = @Name, content = @Content WHERE idtemplate = @Id", entity);
                LogUpdate(entity.Id, "template", "UPDATE template SET name = '" + entity.Name + "', content = '" + entity.Content + "' WHERE idtemplate = " + entity.Id, user.Id);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar la plantilla", ex);
            }
            finally
            {
                CloseConnection();
            }
            return entity;
        }

        /// <summary>
        /// Elimina una plantilla de la base de datos
        /// </summary>
        /// <param name="entity">Plantilla a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Plantilla eliminada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar la plantilla</exception>
        public Template Delete(Template entity, User user)
        {
            try
            {
                OpenConnection();
                _ = _connection.Execute("DELETE FROM template WHERE idtemplate = @Id", entity);
                LogDelete(entity.Id, "template", "DELETE FROM template WHERE idtemplate = " + entity.Id, user.Id);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar la plantilla", ex);
            }
            finally
            {
                CloseConnection();
            }
            return entity;
        }
        #endregion
    }
}
