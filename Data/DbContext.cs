using DomainInterface.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class DbContext : IDbContext
    {
        #region[ CTOR ]

        /// <summary>
        /// Empty constructor
        /// </summary>
        private IConfiguration _configuration;
        protected string ConnectionString { get; set; }

        public DbContext()
        {
            this.ConnectionString = "";// configuration.GetConnectionString("BaseDbKey");
        }


        public DbContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            return connection;
        }



        private DbCommand GetCommand(DbConnection connection, string commandText, CommandType commandType)
        {
            SqlCommand command = new SqlCommand(commandText, connection as SqlConnection);
            command.CommandType = commandType;
            return command;
        }

        public async Task<int> ExecuteNonQuery(string procedureName, 
                                                  List<SqlParameter> parameters, 
                                                  CommandType commandType = CommandType.StoredProcedure)
        {
            int returnValue = -1;
            SqlParameter outParam = new SqlParameter();
            outParam.ParameterName = "RETURN_VALUE";
            outParam.Direction = ParameterDirection.ReturnValue;

            try
            {
                using (SqlConnection connection = this.GetConnection())
                {
                    using (DbCommand cmd = this.GetCommand(connection, procedureName, commandType))
                    {
                        if (parameters != null && parameters.Count > 0)
                        {
                            cmd.Parameters.AddRange(parameters.ToArray());
                        }
                        cmd.Parameters.Add(outParam);

                        returnValue = await cmd.ExecuteNonQueryAsync();
                            returnValue = int.Parse(cmd.Parameters["RETURN_VALUE"].Value.ToString());

                    }
                }
            }
            catch (Exception ex)
            {
                //LogException("Failed to ExecuteNonQuery for " + procedureName, ex, parameters);
                throw;
            }
            return returnValue;
        }



        protected async Task<object> ExecuteScalar(string procedureName, List<SqlParameter> parameters)
        {
            object returnValue = null;
            try
            {
                using (SqlConnection connection = this.GetConnection())
                {
                    using (DbCommand cmd = this.GetCommand(connection, procedureName, CommandType.StoredProcedure))
                    {
                        if (parameters != null && parameters.Count > 0)
                        {
                            cmd.Parameters.AddRange(parameters.ToArray());
                        }
                        returnValue = await cmd.ExecuteScalarAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                //LogException("Failed to ExecuteScalar for " + procedureName, ex, parameters);
                throw;
            }
            return returnValue;

        }



        /// <summary>
        /// GetDataSet
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<DataSet> GetDataSet(string procedureName, 
                                                      List<SqlParameter> parameters, 
                                                      CommandType commandType = CommandType.StoredProcedure)
        {
            string[] s = new string[10];
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection connection = this.GetConnection())
                {
                    using (DbCommand cmd = this.GetCommand(connection, procedureName, commandType))
                    {
                        if (parameters != null && parameters.Count > 0)
                        {
                            cmd.Parameters.AddRange(parameters.ToArray());
                        }               
                        ds.Load(await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection), LoadOption.OverwriteChanges, s);
                    }
                }

            }
            catch (Exception ex)
            {
                //LogException("Failed to GetDataReader for " + procedureName, ex, parameters);
                throw;
            }

            return ds;

        }

        #endregion




    }
}
