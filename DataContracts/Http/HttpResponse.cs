using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DataContracts
{
    public class HttpResponse
    {
        public DateTime requestDateTime = System.DateTime.Now;
        public WebHeaderCollection headers = new WebHeaderCollection();
        public string responseData;
        public bool success = false;
    }
}
