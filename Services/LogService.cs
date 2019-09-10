using DataInterface;
using ServiceInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class LogService : ILogService
    {
        #region[Dependencies]

        //private readonly IConfiguration _config;
        private ICommonRepository _commonRepository;
        private ILogRepository _logRepository;
        public LogService(ILogRepository logRepository,
                            ICommonRepository commonRepository)
        {
            _logRepository = logRepository;
            _commonRepository = commonRepository;
        }


        #endregion

        #region[Dependencies]


        public async Task<int> SaveInterfaceData(int businessInterfaceId, string postedData, string source, string messageType)
        {
            return await _commonRepository.SaveInterfaceData(businessInterfaceId, postedData, source, messageType);


        }


        public async Task<int> SaveLogEntry(int businessInterfaceId, string entry, string interfaceTrackingId)
        {
            return await _logRepository.SaveLogEntry(businessInterfaceId, entry, interfaceTrackingId);

        }



        #endregion

    }
}
