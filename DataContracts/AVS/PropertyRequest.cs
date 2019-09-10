using System;
using System.Collections.Generic;


namespace DataContracts
{
    public class PropertyRequest
    {
        public Guid uniqueID { get; set; }
        public int eventCode { get; set; }
        public LoanInfo loanInfo { get; set; }
        public Address subjectAddress { get; set; }
        public List<Product> products { get; set; }
        public List<Element> elements { get; set; }
        public List<File> files { get; set; }
        public List<Contact> contacts { get; set; }
        public string note { get; set; }
        public decimal customerFee { get; set; }

    }
}
