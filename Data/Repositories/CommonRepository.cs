using DataInterface;
using DomainInterface.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;

namespace Data.Repositories
{
    public class CommonRepository : ICommonRepository
    {
        private IDbContext _dbContext;
        #region [ CTOR ] 

        public CommonRepository(IDbContext context)
        : base()
        {
            _dbContext = context;
        }
        #endregion

        #region [ Public Functions ]


        public async Task<string> GetStateByName(string state)
        {
                Dictionary<string, object> obj = new Dictionary<string, object>();
                string res = "";
                try
                {
                    List<SqlParameter> paramCol = new List<SqlParameter>();
                    SqlParameter p = new SqlParameter("txt_state", SqlDbType.Int);
                    p.Value = state;
                    paramCol.Add(p);
                    DataTable dt = new DataTable();
                    DataSet ds = await _dbContext.GetDataSet("pr_get_State_by_State", paramCol, System.Data.CommandType.StoredProcedure);
                    if (ds.Tables[0] != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            obj =  ds.Tables[0].AsEnumerable().Select(r => r.Table.Columns.Cast<DataColumn>()
                                   .Select(c => new KeyValuePair<string, object>(c.ColumnName, r[c.Ordinal])
                                  ).ToDictionary(z => z.Key, z => z.Value)
                                   ).FirstOrDefault();

                            res = JsonConvert.SerializeObject(obj);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    res = ex.Message;

                }
                return res;

            }


        public async Task<string> GetZipInfo(string zipCode)
        {
            Dictionary<string, object> obj = new Dictionary<string, object>();
            string res = "";
            try
            {
                List<SqlParameter> paramCol = new List<SqlParameter>();
                SqlParameter p = new SqlParameter("zip_code", SqlDbType.VarChar);
                p.Value = zipCode;
                paramCol.Add(p);
                DataTable dt = new DataTable();
                DataSet ds = await _dbContext.GetDataSet("pr_get_zip_code", paramCol, System.Data.CommandType.StoredProcedure);
                if (ds.Tables[0] != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj = ds.Tables[0].AsEnumerable().Select(r => r.Table.Columns.Cast<DataColumn>()
                              .Select(c => new KeyValuePair<string, object>(c.ColumnName.ToLower(), r[c.Ordinal])
                             ).ToDictionary(z => z.Key, z => z.Value)
                               ).FirstOrDefault();

                        res = JsonConvert.SerializeObject(obj);
                    }
                }
            }
            catch (System.Exception ex)
            {
                res = "";

            }
            return res;

        }


        public async Task<string> GetInterfaceLookup(int businessInterfaceId, string fieldName, string fieldValue, int customerAccountId)
        {
            Dictionary<string, object> obj = new Dictionary<string, object>();
            string res = "";
            try
            {
                string spName = "pr_get_business_interface_lookup_by_Field_Name_and_Field_value";
                List<SqlParameter> paramCol = new List<SqlParameter>();
                SqlParameter p = new SqlParameter("txt_Field_Name", SqlDbType.VarChar);
                p.Value = fieldName;
                paramCol.Add(p);

                p = new SqlParameter("txt_Field_Value", SqlDbType.VarChar);
                p.Value = fieldValue;
                paramCol.Add(p);

                p = new SqlParameter("Business_Interface_ID", SqlDbType.Int);
                p.Value = businessInterfaceId;
                paramCol.Add(p);

                if (customerAccountId > 0)
                {
                    p = new SqlParameter("customer_account_id", SqlDbType.Int);
                    p.Value = customerAccountId;
                    paramCol.Add(p);
                    spName = "pr_get_business_interface_lookup_by_Field_Name_and_Field_value_Customer_Account_ID";
                }


                DataTable dt = new DataTable();
                DataSet ds = await _dbContext.GetDataSet(spName, paramCol, System.Data.CommandType.StoredProcedure);
                if (ds.Tables[0] != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj = ds.Tables[0].AsEnumerable().Select(r => r.Table.Columns.Cast<DataColumn>()
                              .Select(c => new KeyValuePair<string, object>(c.ColumnName.ToLower(), r[c.Ordinal])
                             ).ToDictionary(z => z.Key, z => z.Value)
                               ).FirstOrDefault();

                        res = JsonConvert.SerializeObject(obj);
                    }
                }
            }
            catch (System.Exception ex)
            {
                res = "";

            }
            return res;

        }

        public async Task<List<Dictionary<string, object>>> GetInterfaceDataNotProcessed()
        {
            List<Dictionary<string, object>> err = new List<Dictionary<string, object>>();
            try
            {
                List<SqlParameter> paramCol = new List<SqlParameter>();

                SqlParameter p = new SqlParameter("business_interface_id", SqlDbType.Int);
                p.Value = 1;
                paramCol.Add(p);
                DataTable dt = new DataTable();
                DataSet ds = await _dbContext.GetDataSet("proc_get_business_interface_data_not_processed", paramCol, System.Data.CommandType.StoredProcedure);
                if (ds.Tables[0] != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        //jsonStr = ds.Tables[0].Rows[0][0].ToString();
                        return ds.Tables[0].AsEnumerable().Select(r => r.Table.Columns.Cast<DataColumn>()
                               .Select(c => new KeyValuePair<string, object>(c.ColumnName.ToLower(), r[c.Ordinal])
                              ).ToDictionary(z => z.Key, z => z.Value)
                               ).ToList();

                    }
                }
            }
            catch (System.Exception ex)
            {
                err.Add(new Dictionary<string, object>());

            }
            return err;
        }

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

                retval = await _dbContext.ExecuteNonQuery("pr_Save_Business_Interface_Data", paramCol, System.Data.CommandType.StoredProcedure);

                if (retval < 0)
                {


                }
                return retval;
            }
            catch (System.Exception ex)
            {
                return 0;
            }



        }

        public async Task<int> UpdateInterfaceDataProcessed(int businessInterfaceDataId)
        {
            int retval;
            try
            {
                string jsonStr = "";
                List<SqlParameter> paramCol = new List<SqlParameter>();

                SqlParameter p = new SqlParameter("business_interface_data_id", SqlDbType.Int);
                p.Value = businessInterfaceDataId;
                paramCol.Add(p);

                retval = await _dbContext.ExecuteNonQuery("pr_Save_Business_Interface_Data_Processed", paramCol, System.Data.CommandType.StoredProcedure);

                if (retval < 0)
                {


                }
                return retval;
            }
            catch (System.Exception ex)
            {
                return 0;
            }



        }

        
        public async Task<string> GetInterfaceData(int interfaceId)
        {
            Dictionary<string, object> obj = new Dictionary<string, object>();
            string res = "";
            try
            {
                List<SqlParameter> paramCol = new List<SqlParameter>();
                SqlParameter p = new SqlParameter("business_interface_id", SqlDbType.Int);
                p.Value = interfaceId;
                paramCol.Add(p);


                DataTable dt = new DataTable();
                DataSet ds = await _dbContext.GetDataSet("pr_get_business_interface", paramCol, System.Data.CommandType.StoredProcedure);
                if (ds.Tables[0] != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        obj = ds.Tables[0].AsEnumerable().Select(r => r.Table.Columns.Cast<DataColumn>()
                              .Select(c => new KeyValuePair<string, object>(c.ColumnName.ToLower(), r[c.Ordinal])
                             ).ToDictionary(z => z.Key, z => z.Value)
                               ).FirstOrDefault();

                        res = JsonConvert.SerializeObject(obj);
                    }
                }
            }
            catch (System.Exception ex)
            {
                res = "";

            }
            return res;

        }

        #endregion
    }
}
