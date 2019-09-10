using DataContracts;
using System.Collections.Generic;

namespace DataContracts
{
    public class Event120_200 : Request
    {
        public Event120_200()
        {
            this.requestDateTime = System.DateTime.Now;
            this.internalAccountIdentifier = "";
            this.loginAccountIdentifier = "";
            this.loginAccountPassword = "";
            this.requestData = new List<RequestData>();
        }



    }
}
