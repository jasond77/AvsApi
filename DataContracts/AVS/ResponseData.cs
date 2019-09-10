using System;
using System.Collections.Generic;
using System.Text;

namespace DataContracts
{
    public class ResponseData
    {
        public int statusCode;

        public string description { get; set; }
        public string comment { get; set; }
        public List<PropertyResponse> propertyResponse { get; set; }


    }
}
