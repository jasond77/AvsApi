using System;
using System.Collections.Generic;
using System.Text;

namespace DataContracts
{
    public  class Address
    {
        public string streetAddress { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postalCode { get; set; }
        public string county { get; set; }

    }
}
