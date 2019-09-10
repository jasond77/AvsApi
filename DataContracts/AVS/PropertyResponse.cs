using System;
using System.Collections.Generic;


namespace DataContracts
{
    public class PropertyResponse
    {
        public int eventCode { get; set; }
        public Guid uniqueID { get; set; }
        public decimal? customerFee { get; set; }
    }
}
