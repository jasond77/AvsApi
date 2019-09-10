using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface
{
    public interface ILogService
    {


        Task<int> SaveInterfaceData(int businessInterfaceId, string postedData, string source , string messageType);

        Task<int> SaveLogEntry(int businessInterfaceId, string entry, string interfaceTrackingId);




    }
}
