
using System.Threading.Tasks;
namespace DataInterface
{
    public interface ILogRepository
    {


        Task<int> SaveLogEntry(int businessInterfaceId, string entry, string interfaceTrackingId);


    }
}
