using System;
using System.Collections.Generic;
using System.Text;

namespace DataContracts
{
    public class RequestData
    {

        public DateTime eventDate { get; set; }
        public string description { get; set; }
        public string comment { get; set; }
        public List<PropertyRequest> propertyRequest { get; set; }

    }
}
