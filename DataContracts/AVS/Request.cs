
using System;
using System.Collections.Generic;

namespace DataContracts
{
    public class Request
    {
        public DateTime requestDateTime{ get; set; }
        public int accountID { get; set; }
        public string internalAccountIdentifier{ get; set; }
        public string loginAccountIdentifier{ get; set; }
        public string loginAccountPassword{ get; set; }
        public List<RequestData> requestData { get; set; }

    }
}
