using System;
using System.Collections.Generic;
using System.Text;

namespace DataContracts
{
   public class Event140 : Request
    {

        public Event140()
        {
            this.requestDateTime = System.DateTime.Now;
            this.internalAccountIdentifier = "";
            this.loginAccountIdentifier = "";
            this.loginAccountPassword = "";
            this.requestData = new List<RequestData>();
        }
    }
}
