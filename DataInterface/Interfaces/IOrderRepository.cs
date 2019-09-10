
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace DataInterface
{
    public interface IOrderRepository
    {

        Task<string> GetOrderByID(int userId);


        Task<int> SaveProperty(int propertyId, 
                                  string address1, 
                                  string address2, 
                                  string city, 
                                  int state, 
                                  string zipCode,
                                  string county,
                                  int modifiedEmployeeId);

        Task<int> SaveOrder(int orderId,  
                               int propertyId,
                               int customerAccountId,
                               string loanNumber,
                               decimal loanAmount,
                               string loanReference,
                               int loanTypeId,
                               int businessId,
                               int modifiedEmployeeId);

        Task<int> SaveOrderDetail(int orderDetailId,
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
                                                int modifiedEmployeeId);

        Task<string> GetOrderByInterfaceId(int interfaceId,
                                           string interfaceUniqueId);

        Task<string> GetOrderByUniqueId(string orderUniqueId);


        Task<decimal> GetCustomerFee(int customerProductId,
                                     int stateId,
                                     string county);
        Task<string> GetOrderFolder(int orderDetailId);

        Task<int> SaveOrderNote(int orderDetailId,
                                string note,
                                string noteType,
                                int statusId,
                                string userName,
                                string userType,
                                int modifiedEmployeeId);


        Task<int> SaveInterfaceTracking(int orderDetailId,
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
                                        string specialInstructions);

        Task<int> SaveOrderContact(int orderDetailId,
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
                                    DateTime ModifiedDate);


        Task<string> GetInterfaceOrderData(int orderDetailId);



        Task<int> SaveOrderDetailEvent(int orderDetailId,
                                       int baseEventId,
                                       int modifiedEmployeeId);

    }



}
