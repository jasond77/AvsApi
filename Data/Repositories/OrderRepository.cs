using DataInterface;
using DomainInterface.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        #region [ CTOR ] 

        private IDbContext _dbContext;
        readonly IConfiguration _configuration;

        public OrderRepository(DomainInterface.Interfaces.IDbContext context,
                               IConfiguration configuration)
            : base()
        {
            _dbContext = context;
            _configuration = configuration;
        }
        #endregion

        #region [ Public Functions ]

        /// <summary>
        /// Get user by username and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<string> GetOrderByID(int userID)
        {
            try
            {
                string jsonStr = "";
                DataSet ds = await _dbContext.GetDataSet("proc_get_users", null, System.Data.CommandType.StoredProcedure);
                if (ds.Tables[0] != null)
                {
                    if(ds.Tables[0].Rows.Count >0)
                    {
                        jsonStr = ds.Tables[0].Rows[0][0].ToString();
                       // JsonConvert.DeserializeObject<RootObject>(jsonstring
                    }
                }
                return jsonStr;
            }
            catch (System.Exception ex)
            {
                return null;
            }
        }



        public async Task<int> SaveOrderDetail(int orderDetailId,
                                                int orderId,
                                                string orderNumber,
                                                int customerProductId,
                                                int customerEmployeeId,
                                                decimal customerFee,
                                                DateTime dueDate,
                                                DateTime hardDueDate,
                                                DateTime? appointmentDate,
                                                string batchNumber,
                                                int myOrder,
                                                int flagOrder,
                                                DateTime modifiedDate,
                                                string customerOrderNumber,
                                                string lenderName,
                                                string lenderAddress,
                                                string source,
                                                string specialInstructions,
                                                decimal appraisedValue,
                                                string lockBox,
                                                int modifiedEmployeeId)
        {

            int retval;




            try
            {
                List<SqlParameter> paramCol = new List<SqlParameter>();

                

                SqlParameter p = new SqlParameter("modified_employee_id", SqlDbType.Int);
                p.Value = modifiedEmployeeId;
                paramCol.Add(p);

                 p = new SqlParameter("order_detail_id", SqlDbType.Int);
                p.Value = 0;
                paramCol.Add(p);

                 p = new SqlParameter("order_id", SqlDbType.Int);
                p.Value = orderId;
                paramCol.Add(p);

                p = new SqlParameter("txt_Order_Number", SqlDbType.VarChar);
                p.Value = orderNumber;
                paramCol.Add(p);

                p = new SqlParameter("customer_product_id", SqlDbType.Int);
                p.Value = customerProductId;
                paramCol.Add(p);

                p = new SqlParameter("customer_employee_id", SqlDbType.Int);
                p.Value = customerEmployeeId;
                paramCol.Add(p);

                p = new SqlParameter("mny_Customer_Fee", SqlDbType.Decimal);
                p.Value = customerFee;
                paramCol.Add(p);

                p = new SqlParameter("dte_Due_Date", SqlDbType.DateTime);
                p.Value = dueDate;
                paramCol.Add(p);
         
                p = new SqlParameter("dte_hard_due_date", SqlDbType.DateTime);
                p.Value = hardDueDate;
                paramCol.Add(p);

                p = new SqlParameter("dte_Appointment", SqlDbType.DateTime);
                p.Value = null;
                paramCol.Add(p);

                p = new SqlParameter("txt_batch_number", SqlDbType.VarChar);
                p.Value = batchNumber;
                paramCol.Add(p);

                p = new SqlParameter("my_order", SqlDbType.VarChar);
                p.Value = myOrder;
                paramCol.Add(p);
                
                p = new SqlParameter("flag_order", SqlDbType.Int);
                p.Value = flagOrder;
                paramCol.Add(p);

                p = new SqlParameter("dte_modified", SqlDbType.DateTime);
                p.Value = modifiedDate;
                paramCol.Add(p);
                
                p = new SqlParameter("txt_Customer_Order_Number", SqlDbType.VarChar);
                p.Value = customerOrderNumber;
                paramCol.Add(p);

                p = new SqlParameter("txt_Lender_Name", SqlDbType.VarChar);
                p.Value = lenderName;
                paramCol.Add(p);

                p = new SqlParameter("txt_lender_Address", SqlDbType.VarChar);
                p.Value = lenderAddress;
                paramCol.Add(p);

                p = new SqlParameter("txt_source", SqlDbType.VarChar);
                p.Value = source;
                paramCol.Add(p);

                p = new SqlParameter("txt_special_instructions", SqlDbType.VarChar);
                p.Value = specialInstructions;
                paramCol.Add(p);

                p = new SqlParameter("mny_Appraised_Value", SqlDbType.Decimal);
                p.Value = appraisedValue;
                paramCol.Add(p);

                p = new SqlParameter("lockbox", SqlDbType.VarChar);
                p.Value = lockBox;
                paramCol.Add(p);

                p = new SqlParameter("IsFederalHoliday", SqlDbType.Bit);
                p.Value = 0;
                paramCol.Add(p);


                retval = await _dbContext.ExecuteNonQuery("pr_save_order_detail", paramCol, System.Data.CommandType.StoredProcedure);

            }
            catch (System.Exception ex)
            {
                retval = 0;
            }


            return retval;


        }

        public async Task<int> SaveOrder(int orderId,
                                        int propertyId,
                                        int customerAccountId,
                                        string loanNumber,
                                        decimal loanAmount,
                                        string loanReference,
                                        int loanTypeId,
                                        int businessId,
                                        int modifiedEmployeeId)
        {

            int retval;
            try
            {
                List<SqlParameter> paramCol = new List<SqlParameter>();

                SqlParameter p = new SqlParameter("order_id", SqlDbType.Int);
                p.Value = orderId;
                paramCol.Add(p);

                p = new SqlParameter("modified_employee_id", SqlDbType.Int);
                p.Value = modifiedEmployeeId;
                paramCol.Add(p);

                 p = new SqlParameter("property_id", SqlDbType.Int);
                p.Value = propertyId;
                paramCol.Add(p);

                 p = new SqlParameter("business_id", SqlDbType.Int);
                p.Value = businessId;
                paramCol.Add(p);

                 p = new SqlParameter("customer_account_id", SqlDbType.Int);
                p.Value = customerAccountId;
                paramCol.Add(p);

                p = new SqlParameter("txt_Loan_Number", SqlDbType.VarChar);
                p.Value = loanNumber;
                paramCol.Add(p);

                p = new SqlParameter("mny_Loan_Amount", SqlDbType.VarChar);
                p.Value = loanAmount;
                paramCol.Add(p);

                p = new SqlParameter("txt_Loan_reference", SqlDbType.VarChar);
                p.Value = loanAmount;
                paramCol.Add(p);

                p = new SqlParameter("order_loan_type_id", SqlDbType.Int);
                p.Value = loanTypeId;
                paramCol.Add(p);

                p = new SqlParameter("dte_modified", SqlDbType.DateTime);
                p.Value = System.DateTime.Now;
                paramCol.Add(p);


                retval = await _dbContext.ExecuteNonQuery("pr_save_order", paramCol, System.Data.CommandType.StoredProcedure);

            }
            catch (System.Exception ex)
            {
                retval = 0;
            }


            return retval;


        }

        public async Task<int> SaveProperty(int propertyId,
                                  string address1,
                                  string address2,
                                  string city,
                                  int stateId,
                                  string zipCode,
                                  string county,
                                  int modifiedEmployeeId)
        {

            int retval;
            try
            {
                List<SqlParameter> paramCol = new List<SqlParameter>();

                SqlParameter p = new SqlParameter("modified_employee_id", SqlDbType.Int);
                p.Value = modifiedEmployeeId;
                paramCol.Add(p);

                p = new SqlParameter("property_id", SqlDbType.Int);
                p.Value = propertyId;
                paramCol.Add(p);
                
                p = new SqlParameter("txt_Address1", SqlDbType.VarChar);
                p.Value = address1;
                paramCol.Add(p);

                p = new SqlParameter("txt_Address2", SqlDbType.VarChar);
                p.Value = address2;
                paramCol.Add(p);

                p = new SqlParameter("txt_City", SqlDbType.VarChar);
                p.Value = city;
                paramCol.Add(p);

                p = new SqlParameter("State_ID", SqlDbType.Int);
                p.Value = stateId;
                paramCol.Add(p);

                p = new SqlParameter("txt_County", SqlDbType.VarChar);
                p.Value = county;
                paramCol.Add(p);

                p = new SqlParameter("txt_Zip", SqlDbType.VarChar);
                p.Value = zipCode;
                paramCol.Add(p);

                p = new SqlParameter("dte_modified", SqlDbType.DateTime);
                p.Value = System.DateTime.Now;
                paramCol.Add(p);


                retval = await _dbContext.ExecuteNonQuery("pr_save_property", paramCol, System.Data.CommandType.StoredProcedure);

            }
            catch (System.Exception ex)
            {
                retval = 0;
            }


                return retval;


        }


        public async Task<string> GetOrderByInterfaceId(int interfaceId, 
                                                        string interfaceUniqueId)
        {

            Dictionary<string, object> obj = new Dictionary<string, object>();
            string res = "";
            try
            {
                List<SqlParameter> paramCol = new List<SqlParameter>();

                SqlParameter p = new SqlParameter("business_interface_id", SqlDbType.Int);
                p.Value = interfaceId;
                paramCol.Add(p);

                p = new SqlParameter("txt_Interface_Unique_ID", SqlDbType.VarChar);
                p.Value = interfaceUniqueId;
                paramCol.Add(p);

                DataTable dt = new DataTable();

                DataSet ds = await _dbContext.GetDataSet("pr_Get_Order_Detail_By_Interface_Unique_ID", paramCol, System.Data.CommandType.StoredProcedure);
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


        public async Task<string> GetOrderByUniqueId(string orderUniqueId)
        {

            Dictionary<string, object> obj = new Dictionary<string, object>();
            string res = "";
            try
            {
                List<SqlParameter> paramCol = new List<SqlParameter>();

                SqlParameter p = new SqlParameter("unique_id", SqlDbType.VarChar);
                p.Value = orderUniqueId;
                paramCol.Add(p);

                DataTable dt = new DataTable();

                DataSet ds = await _dbContext.GetDataSet("pr_get_order_detail_by_unique_id", paramCol, System.Data.CommandType.StoredProcedure);
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
        
        public async Task<decimal> GetCustomerFee(int customerProductId,
                                                 int stateId,
                                                 string county)
        {

            Dictionary<string, object> obj = new Dictionary<string, object>();
            decimal fee = 0;
            try
            {
                List<SqlParameter> paramCol = new List<SqlParameter>();

                SqlParameter p = new SqlParameter("customer_product_id", SqlDbType.Int);
                p.Value = customerProductId;
                paramCol.Add(p);

                p = new SqlParameter("state_id", SqlDbType.Int);
                p.Value = stateId;
                paramCol.Add(p);

                p = new SqlParameter("txt_County", SqlDbType.VarChar);
                p.Value = county;
                paramCol.Add(p);

                DataTable dt = new DataTable();

                DataSet ds = await _dbContext.GetDataSet("pr_get_customer_product_fee_by_customer_product_id", paramCol, System.Data.CommandType.StoredProcedure);
                if (ds.Tables[0] != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        fee = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                    }
                }
            }
            catch (System.Exception ex)
            {
                fee = 0;

            }
            return fee;

        }




        public async Task<int> SaveOrderNote(int orderDetailId,
                                            string note,
                                            string noteType,
                                            int statusId,
                                            string userName,
                                            string userType,
                                            int modifiedEmployeeId)
        {

            int retval;
            try
            {



        string xmlNote = "<note status=\"" + statusId.ToString() + "\"" 
                          + " created_by=\"" + userName + "\""
                          + " employee_id=\"" + modifiedEmployeeId + "\"" 
                          + " date_created=\"" + System.DateTime.Now.ToString() + "\"" 
                          + " sort_date=\"" + System.DateTime.Now.Ticks.ToString() + "\"" 
                          + " user_type=\"" + userType + "\">" + HttpUtility.HtmlEncode(note) + "</note>";

                List<SqlParameter> paramCol = new List<SqlParameter>();

                SqlParameter p = new SqlParameter("xml_notes", SqlDbType.VarChar);
                p.Value = xmlNote;
                paramCol.Add(p);

                p = new SqlParameter("order_detail_id", SqlDbType.Int);
                p.Value = orderDetailId;
                paramCol.Add(p);

                p = new SqlParameter("note_type", SqlDbType.VarChar);
                p.Value = noteType;
                paramCol.Add(p);


                retval = await _dbContext.ExecuteNonQuery("pr_Save_order_notes", paramCol, System.Data.CommandType.StoredProcedure);

            }
            catch (System.Exception ex)
            {
                retval = 0;
            }


            return retval;


        }

        public async Task<int> SaveInterfaceTracking(int orderDetailId,
                                                    string interfaceUniqueId,
                                                    DateTime lastCustomerNote,
                                                    DateTime inspectionDate,
                                                    bool inspected,
                                                    decimal customerFee,
                                                    DateTime dueDate,
                                                    bool assigned,
                                                    bool fileUploaded,
                                                    int interfaceId,
                                                    string repCode,
                                                    string specialInstructions)

        {

            int retval;
            try
            {

                List<SqlParameter> paramCol = new List<SqlParameter>();

            SqlParameter p = new SqlParameter("order_detail_id", SqlDbType.Int);
            p.Value = orderDetailId;
            paramCol.Add(p);

            p = new SqlParameter("txt_Interface_Unique_ID", SqlDbType.VarChar);
            p.Value = interfaceUniqueId;
            paramCol.Add(p);

            p = new SqlParameter("dte_Last_Customer_Note", SqlDbType.DateTime);
            p.Value = lastCustomerNote;
            paramCol.Add(p);

            p = new SqlParameter("dte_Inspection", SqlDbType.DateTime);
            p.Value = inspectionDate;
            paramCol.Add(p);

            p = new SqlParameter("bit_Inspected", SqlDbType.Bit);
            p.Value = inspected;
            paramCol.Add(p);

            p = new SqlParameter("mny_Customer_Fee", SqlDbType.Money);
            p.Value = customerFee;
            paramCol.Add(p);

            p = new SqlParameter("dte_Due", SqlDbType.DateTime);
            p.Value = dueDate;
            paramCol.Add(p);

            p = new SqlParameter("bit_Assigned", SqlDbType.Bit);
            p.Value = assigned;
            paramCol.Add(p);

            p = new SqlParameter("bit_File_Uploaded", SqlDbType.Bit);
            p.Value = fileUploaded;
            paramCol.Add(p);

            p = new SqlParameter("dte_modified", SqlDbType.DateTime);
            p.Value = DateTime.Now;
            paramCol.Add(p);

            p = new SqlParameter("Business_Interface_ID", SqlDbType.Int);
            p.Value = interfaceId;
            paramCol.Add(p);

            p = new SqlParameter("txt_rep_code", SqlDbType.VarChar);
            p.Value = repCode;
            paramCol.Add(p);

            p = new SqlParameter("txt_special_instructions", SqlDbType.VarChar);
            p.Value = specialInstructions;
            paramCol.Add(p);


            retval = await _dbContext.ExecuteNonQuery("pr_Save_Order_Detail_Interface_Tracking", paramCol, System.Data.CommandType.StoredProcedure);

        }
            catch (System.Exception ex)
            {
                retval = 0;
            }


            return retval;


        }



        public async Task<int> SaveOrderContact(int orderDetailId,
                                                int orderContactId,
                                                string firstName,
                                                string lastName,
                                                string middleName,
                                                string phoneNumber,
                                                string workNumber,
                                                string homeNumber,
                                                string faxNumber,
                                                string emailAddress,
                                                int contactTypeId,
                                                int modifiedEmployeeId,
                                                DateTime ModifiedDate)

        {

            int retval;
            try
            {

                List<SqlParameter> paramCol = new List<SqlParameter>();

                SqlParameter p = new SqlParameter("order_detail_id", SqlDbType.Int);
                p.Value = orderDetailId;
                paramCol.Add(p);

                p = new SqlParameter("Order_contact_ID", SqlDbType.Int);
                p.Value = orderContactId;
                paramCol.Add(p);

                p = new SqlParameter("txt_first_name", SqlDbType.VarChar);
                p.Value = firstName;
                paramCol.Add(p);

                p = new SqlParameter("txt_last_name", SqlDbType.VarChar);
                p.Value = lastName;
                paramCol.Add(p);

                p = new SqlParameter("txt_middle_name", SqlDbType.VarChar);
                p.Value = middleName;
                paramCol.Add(p);

                p = new SqlParameter("txt_phone", SqlDbType.VarChar);
                p.Value = phoneNumber;
                paramCol.Add(p);

                p = new SqlParameter("txt_fax", SqlDbType.VarChar);
                p.Value = faxNumber;
                paramCol.Add(p);

                p = new SqlParameter("txt_email", SqlDbType.VarChar);
                p.Value = emailAddress;
                paramCol.Add(p);

                p = new SqlParameter("contact_type_id", SqlDbType.Int);
                p.Value = contactTypeId;
                paramCol.Add(p);

                p = new SqlParameter("modified_employee_id", SqlDbType.Int);
                p.Value = modifiedEmployeeId;
                paramCol.Add(p);

                p = new SqlParameter("dte_modified", SqlDbType.DateTime);
                p.Value = ModifiedDate;
                paramCol.Add(p);

                p = new SqlParameter("txt_other_phone", SqlDbType.VarChar);
                p.Value = homeNumber;
                paramCol.Add(p);

                p = new SqlParameter("txt_work_phone", SqlDbType.VarChar);
                p.Value = workNumber;
                paramCol.Add(p);

                p = new SqlParameter("int_notifications", SqlDbType.Int);
                p.Value = 0;
                paramCol.Add(p);

                retval = await _dbContext.ExecuteNonQuery("pr_save_order_contact", paramCol, System.Data.CommandType.StoredProcedure);

            }
            catch (System.Exception ex)
            {
                retval = 0;
            }


            return retval;


        }
        

        public async Task<string> GetOrderFolder(int orderDetailId)
        {
            string ret = "";
            try
            {
                List<SqlParameter> paramCol = new List<SqlParameter>();

                SqlParameter p = new SqlParameter("order_detail_id", SqlDbType.Int);
                p.Value = orderDetailId;
                paramCol.Add(p);

                DataTable dt = new DataTable();

                DataSet ds = await _dbContext.GetDataSet("pr_get_order_detail_by_id", paramCol, System.Data.CommandType.StoredProcedure);
                if (ds.Tables[0] != null)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DateTime d = (DateTime)ds.Tables[0].Rows[0]["dte_created"];
                        int month = d.Month;
                        int year = d.Year;
                        ret = _configuration.GetSection("AppSettings:BusinessFolder").Value 
                            + "business" + ds.Tables[0].Rows[0]["business_id"].ToString() 
                            + "\\Orders\\" + month.ToString() + "-" + year.ToString()
                            + "\\" + ds.Tables[0].Rows[0]["txt_customer_id"].ToString() 
                            + "\\" + ds.Tables[0].Rows[0]["unique_id"].ToString()
                            + "\\" + ds.Tables[0].Rows[0]["txt_order_number"].ToString() + "\\";
                        if(System.IO.Directory.Exists(ret) == false)
                        {
                            System.IO.Directory.CreateDirectory(ret);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                ret = "";

            }
            return ret;


        }

        public async Task<string> GetInterfaceOrderData(int orderDetailId)
        {
            Dictionary<string, object> obj = new Dictionary<string, object>();
            string res = "";
            try
            {
                List<SqlParameter> paramCol = new List<SqlParameter>();
                SqlParameter p = new SqlParameter("order_detail_id", SqlDbType.Int);
                p.Value = orderDetailId;
                paramCol.Add(p);


                DataTable dt = new DataTable();
                DataSet ds = await _dbContext.GetDataSet("pr_get_Order_Detail_Interface_by_Order_Detail_ID", paramCol, System.Data.CommandType.StoredProcedure);
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



        public async Task<int> SaveOrderDetailEvent(int orderDetailId,
                                       int baseEventId,
                                       int modifiedEmployeeId)
        {


            int retval;
            try
            {

                List<SqlParameter> paramCol = new List<SqlParameter>();

                SqlParameter p = new SqlParameter("order_detail_id", SqlDbType.Int);
                p.Value = orderDetailId;
                paramCol.Add(p);

                p = new SqlParameter("base_event_id", SqlDbType.Int);
                p.Value = baseEventId;
                paramCol.Add(p);

                p = new SqlParameter("modified_employee_id", SqlDbType.Int);
                p.Value = modifiedEmployeeId;
                paramCol.Add(p);

                retval = await _dbContext.ExecuteNonQuery("pr_save_order_detail_event_manual", paramCol, System.Data.CommandType.StoredProcedure);

            }
            catch (System.Exception ex)
            {
                retval = 0;
            }


            return retval;

        }


        #endregion
    }
}
