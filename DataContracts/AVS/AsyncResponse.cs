using System;
using System.Collections.Generic;
using System.Text;

namespace DataContracts
{
    public class AsyncResponse : Response
    {
        public AsyncResponse()
        {
            this.responseDateTime = System.DateTime.Now;
            this.responseData = new List<ResponseData>();

        }

    }
}
