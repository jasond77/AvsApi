using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace DomainInterface.Interfaces
{
    public interface IDbContext

    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        Task<DataSet> GetDataSet(string procedureName,
                                 List<SqlParameter> parameters,
                                 CommandType commandType = CommandType.StoredProcedure);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        Task<int> ExecuteNonQuery(string procedureName,
                                                  List<SqlParameter> parameters,
                                                  CommandType commandType = CommandType.StoredProcedure);
    }
}
