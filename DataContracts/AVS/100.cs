using System;
using System.Collections.Generic;

namespace DataContracts
{
    public class Event100 : Request
    {
        
        public Event100()
        {
            this.requestDateTime = System.DateTime.Now;
            this.accountID = 0;
            this.internalAccountIdentifier = "";
            this.loginAccountIdentifier = "";
            this.loginAccountPassword = "";
            this.requestData = new List<RequestData>();
        }

    }
}
