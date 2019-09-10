//using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataContracts
{
    public class Response 
    {
        public DateTime responseDateTime { get; set; }
        public string internalAccountIdentifier { get; set; }
        public string loginAccountIdentifier { get; set; }
        public string loginAccountPassword { get; set; }
        public List<ResponseData> responseData { get; set; }

    }
}
