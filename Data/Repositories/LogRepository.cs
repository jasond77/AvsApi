using DataInterface;
using DomainInterface.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class LogRepository : ILogRepository
    {
        #region [ CTOR ] 

        private IDbContext _dbContext;

        public LogRepository(DomainInterface.Interfaces.IDbContext context)
            : base()
        {
            _dbContext = context;
        }
        #endregion

        #region [ Public Functions ]


        public async Task<int> SaveInterfaceData(int businessInterfaceId, string entry, string source, string interfaceTrackingId)
        {
            int retval;
            try
            {
                string jsonStr = "";
                List<SqlParameter> paramCol = new List<SqlParameter>();

                SqlParameter p = new SqlParameter("business_interface_id", SqlDbType.Int);
                p.Value = businessInterfaceId;
                paramCol.Add(p);

                p = new SqlParameter("txt_Post_Data", SqlDbType.VarChar);
                p.Value = entry;
                 paramCol.Add(p);

                p = new SqlParameter("txt_Source", SqlDbType.VarChar);
                p.Value = source;
                paramCol.Add(p);

                p = new SqlParameter("txt_message_type", SqlDbType.VarChar);
                p.Value = interfaceTrackingId;
                paramCol.Add(p);

                retval =  await _dbContext.ExecuteNonQuery("pr_Save_Business_Interface_Data", paramCol, System.Data.CommandType.StoredProcedure);

                if(retval < 0)
                {


                }
                return retval;
            }
            catch (System.Exception ex)
            {
                return 0;
            }



        }

        public async Task<int> SaveLogEntry(int businessInterfaceId, string entry, string interfaceTrackingId)
        {

            try
            {
                string jsonStr = "";
                List<SqlParameter> paramCol = new List<SqlParameter>();

                SqlParameter p = new SqlParameter("business_interface_id", SqlDbType.Int);
                p.Value = businessInterfaceId;
                paramCol.Add(p);

                p = new SqlParameter("txt_Message", SqlDbType.VarChar);
                p.Value = entry;
                paramCol.Add(p);

                p = new SqlParameter("Interface_Unique_ID", SqlDbType.VarChar);
                p.Value = interfaceTrackingId;
                paramCol.Add(p);

                int retval = await _dbContext.ExecuteNonQuery("pr_save_business_interface_log", paramCol, System.Data.CommandType.StoredProcedure);

                if (retval > 0)
                {


                }
                return retval;
            }
            catch (System.Exception ex)
            {
                return 0;
            }
        }



        #endregion
    }
}
