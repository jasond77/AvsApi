using DataContracts;
using System.Collections.Generic;

namespace DataContracts
{
    public class Event130 : Request
    {


        public Event130()
        {
            this.requestDateTime = System.DateTime.Now;
            this.internalAccountIdentifier = "";
            this.loginAccountIdentifier = "";
            this.loginAccountPassword = "";
            this.requestData = new List<RequestData>();

        }


    }
}
