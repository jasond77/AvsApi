

namespace DataContracts
{
    public class Contact
    {


        public string type { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string telephoneNumber1 { get; set; }
        public string telephoneNumber2 { get; set; }
        public string email { get; set; }
        public string faxNumber { get; set; }
        public string birthDate { get; set; }
        public string maritalStatus { get; set; }
        public string ssn { get; set; }
        public Address address { get; set; }


    }
}
