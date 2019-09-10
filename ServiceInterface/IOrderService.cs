using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface
{
    public interface IOrderService
    {

        /// <summary>
        /// Returns a UserDTO by userID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns>UserDTO</returns>
        Task<string> GetOrder(int userID);

        void ProcessInterfaceData(int? interfaceDataId = null);

        Task<string> GetOrderInterfaceData(int orderDetailId);

        Task<string> GetOrderFolder(int orderDetailId);

        Task<int> SaveOrderNote(int orderDetailId,
                                            string note,
                                            string noteType,
                                            int statusId,
                                            string userName,
                                            string userType,
                                            int modifiedEmployeeId);

    }
}
