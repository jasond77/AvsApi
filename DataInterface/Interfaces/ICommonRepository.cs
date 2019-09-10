using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataInterface
{
    public interface ICommonRepository
    {

        Task<string> GetStateByName(string state);


        Task<string> GetInterfaceLookup(int businessInterfaceId, string fieldName, string fieldValue, int customerAccountId);

        Task<string> GetZipInfo(string zipCode);

        Task<List<Dictionary<string, object>>> GetInterfaceDataNotProcessed();

        Task<int> SaveInterfaceData(int businessInterfaceId, string postedData, string source, string messageType);

        Task<int> UpdateInterfaceDataProcessed(int businessInterfaceDataId);

        Task<string> GetInterfaceData(int interfaceId);
    }
}
