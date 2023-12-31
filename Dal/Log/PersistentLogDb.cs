﻿using Dal.Dto;
using Dal.Exceptions;
using Dal.Utils;
using Dapper;
using Entities.Log;
using MySql.Data.MySqlClient;
using System.Data;

namespace Dal.Log
{
    /// <summary>
    /// Realiza la persistencia de los registros de base de datos en la base de datos
    /// </summary>
    public class PersistentLogDb : PersistentBase<LogDb>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la bse de datos
        /// </summary>
        /// <param name="connString">Cadena de conexión a la base de datos</param>
        public PersistentLogDb(string connString) : base(connString) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de registros de base de datos desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de registros</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los registros</exception>
        public override ListResult<LogDb> List(string filters, string orders, int limit, int offset)
        {
            ListResult<LogDb> result;
            try
            {
                QueryBuilder queryBuilder = new("idlog, date, action, idtable, `table`, `sql`, iduser", "log_db");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<LogDb> logs = connection.Query<LogDb>(queryBuilder.GetSelectForList(filters, orders, limit, offset)).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                result = new(logs, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de registros", ex);
            }
            return result;
        }

        /// <summary>
        /// Consulta un registro dado su identificador
        /// </summary>
        /// <param name="entity">Registro a consultar</param>
        /// <returns>Registro con los datos cargados desde la base de datos o datos por defecto si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar el registro</exception>
        public override LogDb Read(LogDb entity)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                LogDb result = connection.QuerySingleOrDefault<LogDb>("SELECT idlog, date, action, idtable, `table`, `sql`, iduser FROM log_db WHERE idlog = @Id", entity);
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
                throw new PersistentException("Error al consultar el registro", ex);
            }
            return entity;
        }

        /// <summary>
        /// Inserta un registro en la base de datos
        /// </summary>
        /// <param name="entity">Registro a insertar</param>
        /// <returns>Registro insertado con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar el registro</exception>
        public override LogDb Insert(LogDb entity)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                entity.Id = connection.QuerySingle<int>(
                    "INSERT INTO log_db (date, action, idtable, `table`, `sql`, iduser) VALUES (NOW(), @Action, @IdTable, @Table, @Sql, @User); SELECT LAST_INSERT_ID();",
                    entity);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar el registro", ex);
            }
            return entity;
        }

        /// <summary>
        /// No hace nada con la actualización de un log
        /// </summary>
        /// <param name="entity">Log a actualizar</param>
        /// <returns>Log actualizado</returns>
        public override LogDb Update(LogDb entity)
        {
            return entity;
        }

        /// <summary>
        /// No hace nada con la eliminación de un log
        /// </summary>
        /// <param name="entity">Log a eliminar</param>
        /// <returns>Log eliminado</returns>
        public override LogDb Delete(LogDb entity)
        {
            return entity;
        }
        #endregion
    }
}