using DataContracts;
using DataInterface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class MessageService : IMessageService
    {
        #region[Dependencies]

        //private readonly IConfiguration _config;
        private ICommonRepository _commonRepository;
        private IHttpService _httpService;
        private ILogService _logService;
        public MessageService(IHttpService httpService,
                             ILogService logService,
                             ICommonRepository commonRepository)
        {
            _logService = logService;
            _commonRepository = commonRepository;
            _httpService = httpService;
        }


        #endregion

        #region[Public Methods]

        public void Send100Event(string url,
                        int interfaceId,
                        string interfaceUniqueId,
                        string orderUniqueId,
                        string userName,
                        string password)
        {
            int retval = 0;
            Task<int> res;
            try
            {
                string log = "Send 100 - " + orderUniqueId + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
                res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);

                HttpResponse httpResponse = new HttpResponse();
                Event100 request = new Event100();
                RequestData requestData = new RequestData();
                PropertyRequest propertyRequest = new PropertyRequest();
                Contact contact = new Contact();
                Product product = new Product();
                Address address = new Address();
                LoanInfo loanInfo = new LoanInfo();
                Element element = new Element();
                File file = new File();

                request.accountID = 1000;
                request.internalAccountIdentifier = "12345";
                request.loginAccountIdentifier = "username";
                request.loginAccountPassword = "password";
                requestData.comment = "comment for new order";
                requestData.description = "new interface order from 123 bank";
                requestData.eventDate = System.DateTime.Now;
                requestData.propertyRequest = new List<PropertyRequest>();

                propertyRequest.contacts = new List<Contact>();
                propertyRequest.eventCode = 100;
                contact = new Contact();
                contact.firstName = "jason";
                contact.lastName = "test";
                contact.telephoneNumber1 = "412-555-1212";
                contact.telephoneNumber2 = "412-555-2121";
                contact.type = "Borrower";
                contact.address = new Address();
                contact.address.streetAddress = "123 First Ave.";
                contact.address.city = "Pittsburgh";
                contact.address.state = "PA";
                contact.address.county = "Allegheny";
                contact.address.postalCode = "15241";
                propertyRequest.contacts.Add(contact);

                propertyRequest.products = new List<Product>();
                product.code = "1122";
                product.name = "FNMA 20000 Desk Review";
                propertyRequest.products.Add(product);

                propertyRequest.subjectAddress = new Address();
                address.streetAddress = "123 Main St.";
                address.city = "Pittsburgh";
                address.state = "PA";
                address.county = "Allegheny";
                address.postalCode = "15241";
                propertyRequest.subjectAddress = address;

                propertyRequest.loanInfo = new LoanInfo();
                loanInfo.agencyCaseIdentifier = "ES-1234";
                loanInfo.appraisedValue = 120000;
                loanInfo.lenderCaseIdentifier = "Loan#123";
                loanInfo.listPrice = 125000;
                loanInfo.LoanAmount = 80000;
                loanInfo.purchasePrice = 110000;
                loanInfo.salePrice = 100000;
                loanInfo.type = "FHA";
                propertyRequest.loanInfo = loanInfo;

                propertyRequest.files = new List<File>();
                file.document = "ASDFGHUYTRERTYJ";
                file.encodingType = "Base64";
                file.extension = "pdf";
                file.name = "FieldReview.pdf";
                file.type = "PDF";
                propertyRequest.files.Add(file);

                propertyRequest.elements = new List<Element>();
                element.name = "Custom Field";
                element.value = "Can be anything you need";
                element.name = "CostCenter";
                element.value = "example of a custom named field";
                requestData.propertyRequest.Add(propertyRequest);

                request.requestData.Add(requestData);
                var payload = new { request };
                JsonSerializerSettings jss = new JsonSerializerSettings();
                jss.NullValueHandling = NullValueHandling.Ignore;
                httpResponse = _httpService.PostMessage(url,
                                         JsonConvert.SerializeObject(payload, jss),
                                         "",
                                         "");
                if (httpResponse.success == true)
                {
                    //check status code
                    JToken avsRequest = JToken.Parse(httpResponse.responseData);
                    int.TryParse(avsRequest["response"]["responseData"][0]["statusCode"].ToString(), out retval);
                    if (retval == 0)
                    {
                        retval = 1;
                        res = _commonRepository.UpdateInterfaceDataProcessed(interfaceId);

                    }
                    else
                    {
                        res = _commonRepository.SaveInterfaceData(interfaceId,
                                                                  JsonConvert.SerializeObject(payload, jss),
                                                                  "App",
                                                                  "Resend");
                    }

                }
            }
            catch (Exception ex)
            {
                string log = "Send 100 error - " + ex.Message + "\n" + orderUniqueId + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
                res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);
                retval = -1;
            }


        }



        public void Send110Event(string url,
                                int interfaceId,
                                string interfaceUniqueId,
                                string orderUniqueId,
                                string userName,
                                string password,
                                decimal customerFee)
        {
            int retval = 0;
            Task<int> res;
            try
            {
                string log = "Send 110 - " + orderUniqueId + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
                res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);

                HttpResponse httpResponse = new HttpResponse();
                Event110 avsResponse = new Event110();
                avsResponse.responseDateTime = System.DateTime.Now;
                avsResponse.internalAccountIdentifier = interfaceUniqueId;
                avsResponse.loginAccountIdentifier = userName;
                avsResponse.loginAccountPassword = password;
                avsResponse.responseData = new List<ResponseData>();
                avsResponse.responseData.Add(new ResponseData());
                avsResponse.responseData[0].comment = "New Order Created";
                avsResponse.responseData[0].description = "";
                avsResponse.responseData[0].statusCode = 0;
                avsResponse.responseData[0].propertyResponse = new List<PropertyResponse>();
                avsResponse.responseData[0].propertyResponse.Add(new PropertyResponse());
                avsResponse.responseData[0].propertyResponse[0].uniqueID = new Guid(orderUniqueId);
                avsResponse.responseData[0].propertyResponse[0].eventCode = 110;
                avsResponse.responseData[0].propertyResponse[0].customerFee = customerFee;
                var payload = new { response = avsResponse };
                JsonSerializerSettings jss= new JsonSerializerSettings();
                jss.NullValueHandling = NullValueHandling.Ignore;
                httpResponse = _httpService.PostMessage(url,
                                         JsonConvert.SerializeObject(payload, jss),
                                         "",
                                         "");
                if (httpResponse.success == true)
                {
                    //check status code
                    JToken avsRequest = JToken.Parse(httpResponse.responseData);
                    int.TryParse(avsRequest["response"]["responseData"][0]["statusCode"].ToString(), out retval);
                    if(retval == 0)
                    {
                        retval = 1;
                        res = _commonRepository.UpdateInterfaceDataProcessed(interfaceId);

                    }
                    else
                    {
                        res = _commonRepository.SaveInterfaceData(interfaceId,
                                                                  JsonConvert.SerializeObject(payload, jss),
                                                                  "App",
                                                                  "Resend");
                    }

                }
            }
            catch (Exception ex)
            {
                string log = "Send 110 error - " + ex.Message + "\n" + orderUniqueId + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
                res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);
                retval = -1;
            }


        }


        public void Send115Event(string url,
                                 int interfaceId,
                                 string interfaceUniqueId,
                                 string userName,
                                 string password,
                                 string comment,
                                 string description)
        {
            int retval = 0;
            Task<int> res;
            try
            {
                string log = "Send 115 - " + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
                res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);

                HttpResponse httpResponse = new HttpResponse();
                Event110 avsResponse = new Event110();
                avsResponse.responseDateTime = System.DateTime.Now;
                avsResponse.internalAccountIdentifier = interfaceUniqueId;
                avsResponse.loginAccountIdentifier = userName;
                avsResponse.loginAccountPassword = password;
                avsResponse.responseData = new List<ResponseData>();
                avsResponse.responseData.Add(new ResponseData());
                avsResponse.responseData[0].comment = comment;
                avsResponse.responseData[0].description = description;
                avsResponse.responseData[0].statusCode = -1;
                avsResponse.responseData[0].propertyResponse = new List<PropertyResponse>();
                avsResponse.responseData[0].propertyResponse.Add(new PropertyResponse());
                avsResponse.responseData[0].propertyResponse[0].eventCode = 115;
                var payload = new { response = avsResponse };
                JsonSerializerSettings jss = new JsonSerializerSettings();
                jss.NullValueHandling = NullValueHandling.Ignore;
                httpResponse = _httpService.PostMessage(url,
                                         JsonConvert.SerializeObject(payload, jss),
                                         "",
                                         "");
                if (httpResponse.success == true)
                {
                    //check status code
                    JToken avsRequest = JToken.Parse(httpResponse.responseData);
                    int.TryParse(avsRequest["response"]["responseData"][0]["statusCode"].ToString(), out retval);
                    if (retval == 0)
                    {
                        retval = 1;
                    }
                    else
                    {
                        res = _commonRepository.SaveInterfaceData(interfaceId,
                                                                  JsonConvert.SerializeObject(payload, jss),
                                                                  "App",
                                                                  "Resend");


                    }
                }
            }
            catch (Exception ex)
            {
                string log = "Send 115 error - " + ex.Message + "\n" + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
                res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);
                retval = -1;
            }

        }


        public void Send120Event(string url,
                                 int interfaceId,
                                 string interfaceUniqueId,
                                 string orderUniqueId,
                                 string userName,
                                 string password,
                                 int accountID,
                                 DateTime eventDate,
                                 string note)
        {
            int retval = 0;
            Task<int> res;
            try
            {
                string log = "Send 120 - " + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
                res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);

                HttpResponse httpResponse = new HttpResponse();
                Event120_200 avsRequest = new Event120_200();
                avsRequest.requestDateTime = System.DateTime.Now;
                avsRequest.internalAccountIdentifier = interfaceUniqueId;
                avsRequest.accountID = accountID;
                avsRequest.loginAccountIdentifier = userName;
                avsRequest.loginAccountPassword = password;
                avsRequest.requestData = new List<RequestData>();
                avsRequest.requestData.Add(new RequestData());
                avsRequest.requestData[0].eventDate = eventDate;
                avsRequest.requestData[0].comment = "";
                avsRequest.requestData[0].description = "";
                avsRequest.requestData[0].propertyRequest = new List<PropertyRequest>();
                avsRequest.requestData[0].propertyRequest.Add(new PropertyRequest());
                avsRequest.requestData[0].propertyRequest[0].eventCode = 120;
                avsRequest.requestData[0].propertyRequest[0].uniqueID = new Guid(orderUniqueId);
                avsRequest.requestData[0].propertyRequest[0].note = note;
                var payload = new { request = avsRequest };
                JsonSerializerSettings jss = new JsonSerializerSettings();
                jss.NullValueHandling = NullValueHandling.Ignore;
                httpResponse = _httpService.PostMessage(url,
                                         JsonConvert.SerializeObject(payload, jss),
                                         "",
                                         "");
                if (httpResponse.success == true)
                {
                    //check status code
                    JToken avsResponse = JToken.Parse(httpResponse.responseData);
                    int.TryParse(avsResponse["response"]["responseData"][0]["statusCode"].ToString(), out retval);
                    if (retval == 0)
                    {
                        retval = 1;
                    }
                    else
                    {
                        res = _commonRepository.SaveInterfaceData(interfaceId,
                                                                  JsonConvert.SerializeObject(payload, jss),
                                                                  "App",
                                                                  "Resend");


                    }
                }
            }
            catch (Exception ex)
            {
                string log = "Send 120 error - " + ex.Message + "\n" + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
                res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);
                retval = -1;
            }

        }

        public void Send121Event(string url,
                          int interfaceId,
                          string interfaceUniqueId,
                          string orderUniqueId,
                          string userName,
                          string password,
                          int accountID,
                          DateTime eventDate,
                          string note)
        {
            int retval = 0;
            Task<int> res;
            try
            {
                string log = "Send 121 - " + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
                res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);

                HttpResponse httpResponse = new HttpResponse();
                Event120_200 avsRequest = new Event120_200();
                avsRequest.requestDateTime = System.DateTime.Now;
                avsRequest.internalAccountIdentifier = interfaceUniqueId;
                avsRequest.accountID = accountID;
                avsRequest.loginAccountIdentifier = userName;
                avsRequest.loginAccountPassword = password;
                avsRequest.requestData = new List<RequestData>();
                avsRequest.requestData.Add(new RequestData());
                avsRequest.requestData[0].eventDate = eventDate;
                avsRequest.requestData[0].comment = "This order is assigned.";
                avsRequest.requestData[0].description = "";
                avsRequest.requestData[0].propertyRequest = new List<PropertyRequest>();
                avsRequest.requestData[0].propertyRequest.Add(new PropertyRequest());
                avsRequest.requestData[0].propertyRequest[0].eventCode = 121;
                avsRequest.requestData[0].propertyRequest[0].uniqueID = new Guid(orderUniqueId);
                avsRequest.requestData[0].propertyRequest[0].note = note;
                var payload = new { request = avsRequest };
                JsonSerializerSettings jss = new JsonSerializerSettings();
                jss.NullValueHandling = NullValueHandling.Ignore;
                httpResponse = _httpService.PostMessage(url,
                                         JsonConvert.SerializeObject(payload, jss),
                                         "",
                                         "");
                if (httpResponse.success == true)
                {
                    //check status code
                    JToken avsResponse = JToken.Parse(httpResponse.responseData);
                    int.TryParse(avsResponse["response"]["responseData"][0]["statusCode"].ToString(), out retval);
                    if (retval == 0)
                    {
                        retval = 1;
                    }
                    else
                    {
                        res = _commonRepository.SaveInterfaceData(interfaceId,
                                                                  JsonConvert.SerializeObject(payload, jss),
                                                                  "App",
                                                                  "Resend");


                    }
                }
            }
            catch (Exception ex)
            {
                string log = "Send 121 error - " + ex.Message + "\n" + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
                res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);
                retval = -1;
            }

        }


        /// <summary>
        /// /
        /// </summary>
        /// <param name="url"></param>
        /// <param name="interfaceId"></param>
        /// <param name="interfaceUniqueId"></param>
        /// <param name="orderUniqueId"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="accountID"></param>
        /// <param name="eventDate"></param>
        /// <param name="documents"></param>
        public void Send130Event(string url,
                                 int interfaceId,
                                 string interfaceUniqueId,
                                 string orderUniqueId,
                                 string userName,
                                 string password,
                                 int accountID,
                                 DateTime eventDate,
                                List<File> documents)
        {
            int retval = 0;
            Task<int> res;
            try
            {
                string log = "Send 130 - " + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
                res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);

                HttpResponse httpResponse = new HttpResponse();
                Event120_200 avsRequest = new Event120_200();
               
                avsRequest.requestDateTime = System.DateTime.Now;
                avsRequest.internalAccountIdentifier = interfaceUniqueId;
                avsRequest.accountID = accountID;
                avsRequest.loginAccountIdentifier = userName;
                avsRequest.loginAccountPassword = password;
                avsRequest.requestData = new List<RequestData>();
                avsRequest.requestData.Add(new RequestData());
                avsRequest.requestData[0].eventDate = eventDate;
                avsRequest.requestData[0].comment = "";
                avsRequest.requestData[0].description = "";
                avsRequest.requestData[0].propertyRequest = new List<PropertyRequest>();
                avsRequest.requestData[0].propertyRequest.Add(new PropertyRequest());
                avsRequest.requestData[0].propertyRequest[0].eventCode = 130;
                avsRequest.requestData[0].propertyRequest[0].uniqueID = new Guid(orderUniqueId);
                avsRequest.requestData[0].propertyRequest[0].files = documents;
                var payload = new { request = avsRequest };
                JsonSerializerSettings jss = new JsonSerializerSettings();
                jss.NullValueHandling = NullValueHandling.Ignore;
                httpResponse = _httpService.PostMessage(url,
                                         JsonConvert.SerializeObject(payload, jss),
                                         "",
                                         "");
                if (httpResponse.success == true)
                {
                    //check status code
                    JToken avsResponse = JToken.Parse(httpResponse.responseData);
                    int.TryParse(avsResponse["response"]["responseData"][0]["statusCode"].ToString(), out retval);
                    if (retval == 0)
                    {
                        retval = 1;
                    }
                    else
                    {
                        res = _commonRepository.SaveInterfaceData(interfaceId,
                                                                  JsonConvert.SerializeObject(payload, jss),
                                                                  "App",
                                                                  "Resend");


                    }
                }
            }
            catch (Exception ex)
            {
                string log = "Send 130 error - " + ex.Message + "\n" + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
                res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);
                retval = -1;
            }

        }

        public void Send140Event(string url,
                         int interfaceId,
                         string interfaceUniqueId,
                         string orderUniqueId,
                         string userName,
                         string password,
                         int accountID,
                         DateTime eventDate,
                         List<File> documents)
        {
            int retval = 0;
            Task<int> res;
            try
            {
                string log = "Send 140 - " + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
                res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);

                HttpResponse httpResponse = new HttpResponse();
                Event120_200 avsRequest = new Event120_200();


                avsRequest.requestDateTime = System.DateTime.Now;
                avsRequest.internalAccountIdentifier = interfaceUniqueId;
                avsRequest.accountID = accountID;
                avsRequest.loginAccountIdentifier = userName;
                avsRequest.loginAccountPassword = password;
                avsRequest.requestData = new List<RequestData>();
                avsRequest.requestData.Add(new RequestData());
                avsRequest.requestData[0].eventDate = eventDate;
                avsRequest.requestData[0].comment = "";
                avsRequest.requestData[0].description = "";
                avsRequest.requestData[0].propertyRequest = new List<PropertyRequest>();
                avsRequest.requestData[0].propertyRequest.Add(new PropertyRequest());
                avsRequest.requestData[0].propertyRequest[0].eventCode = 140;
                avsRequest.requestData[0].propertyRequest[0].uniqueID = new Guid(orderUniqueId);
                avsRequest.requestData[0].propertyRequest[0].files = documents;
                var payload = new { request = avsRequest };
                JsonSerializerSettings jss = new JsonSerializerSettings();
                jss.NullValueHandling = NullValueHandling.Ignore;
                httpResponse = _httpService.PostMessage(url,
                                         JsonConvert.SerializeObject(payload, jss),
                                         "",
                                         "");
                if (httpResponse.success == true)
                {
                    //check status code
                    JToken avsResponse = JToken.Parse(httpResponse.responseData);
                    int.TryParse(avsResponse["response"]["responseData"][0]["statusCode"].ToString(), out retval);
                    if (retval == 0)
                    {
                        retval = 1;
                    }
                    else
                    {
                        res = _commonRepository.SaveInterfaceData(interfaceId,
                                                                  JsonConvert.SerializeObject(payload, jss),
                                                                  "App",
                                                                  "Resend");


                    }
                }
            }
            catch (Exception ex)
            {
                string log = "Send 140 error - " + ex.Message + "\n" + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
                res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);
                retval = -1;
            }

        }



        public void Send150Event(string url,
                         int interfaceId,
                         string interfaceUniqueId,
                         string orderUniqueId,
                         string userName,
                         string password,
                         int accountID,
                         DateTime eventDate,
                         string note)
        {
            int retval = 0;
            Task<int> res;
            try
            {
                string log = "Send 150 - " + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
                res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);

                HttpResponse httpResponse = new HttpResponse();
                Event120_200 avsRequest = new Event120_200();
                avsRequest.requestDateTime = System.DateTime.Now;
                avsRequest.internalAccountIdentifier = interfaceUniqueId;
                avsRequest.accountID = accountID;
                avsRequest.loginAccountIdentifier = userName;
                avsRequest.loginAccountPassword = password;
                avsRequest.requestData = new List<RequestData>();
                avsRequest.requestData.Add(new RequestData());
                avsRequest.requestData[0].eventDate = eventDate;
                avsRequest.requestData[0].comment = "";
                avsRequest.requestData[0].description = "";
                avsRequest.requestData[0].propertyRequest = new List<PropertyRequest>();
                avsRequest.requestData[0].propertyRequest.Add(new PropertyRequest());
                avsRequest.requestData[0].propertyRequest[0].eventCode = 150;
                avsRequest.requestData[0].propertyRequest[0].uniqueID = new Guid(orderUniqueId);
                avsRequest.requestData[0].propertyRequest[0].note = note;
                var payload = new { request = avsRequest };
                JsonSerializerSettings jss = new JsonSerializerSettings();
                jss.NullValueHandling = NullValueHandling.Ignore;
                httpResponse = _httpService.PostMessage(url,
                                         JsonConvert.SerializeObject(payload, jss),
                                         "",
                                         "");
                if (httpResponse.success == true)
                {
                    //check status code
                    JToken avsResponse = JToken.Parse(httpResponse.responseData);
                    int.TryParse(avsResponse["response"]["responseData"][0]["statusCode"].ToString(), out retval);
                    if (retval == 0)
                    {
                        retval = 1;
                    }
                    else
                    {
                        res = _commonRepository.SaveInterfaceData(interfaceId,
                                                                  JsonConvert.SerializeObject(payload, jss),
                                                                  "App",
                                                                  "Resend");


                    }
                }
            }
            catch (Exception ex)
            {
                string log = "Send 150 error - " + ex.Message + "\n" + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
                res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);
                retval = -1;
            }

        }



        public void Send160Event(string url,
                        int interfaceId,
                        string interfaceUniqueId,
                        string orderUniqueId,
                        string userName,
                        string password,
                        int accountID,
                        DateTime eventDate,
                        string note)
        {
            int retval = 0;
            Task<int> res;
            try
            {
                string log = "Send 160 - " + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
                res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);

                HttpResponse httpResponse = new HttpResponse();
                Event120_200 avsRequest = new Event120_200();
                avsRequest.requestDateTime = System.DateTime.Now;
                avsRequest.internalAccountIdentifier = interfaceUniqueId;
                avsRequest.accountID = accountID;
                avsRequest.loginAccountIdentifier = userName;
                avsRequest.loginAccountPassword = password;
                avsRequest.requestData = new List<RequestData>();
                avsRequest.requestData.Add(new RequestData());
                avsRequest.requestData[0].eventDate = eventDate;
                avsRequest.requestData[0].comment = "";
                avsRequest.requestData[0].description = "";
                avsRequest.requestData[0].propertyRequest = new List<PropertyRequest>();
                avsRequest.requestData[0].propertyRequest.Add(new PropertyRequest());
                avsRequest.requestData[0].propertyRequest[0].eventCode = 160;
                avsRequest.requestData[0].propertyRequest[0].uniqueID = new Guid(orderUniqueId);
                avsRequest.requestData[0].propertyRequest[0].note = note;
                var payload = new { request = avsRequest };
                JsonSerializerSettings jss = new JsonSerializerSettings();
                jss.NullValueHandling = NullValueHandling.Ignore;
                httpResponse = _httpService.PostMessage(url,
                                         JsonConvert.SerializeObject(payload, jss),
                                         "",
                                         "");
                if (httpResponse.success == true)
                {
                    //check status code
                    JToken avsResponse = JToken.Parse(httpResponse.responseData);
                    int.TryParse(avsResponse["response"]["responseData"][0]["statusCode"].ToString(), out retval);
                    if (retval == 0)
                    {
                        retval = 1;
                    }
                    else
                    {
                        res = _commonRepository.SaveInterfaceData(interfaceId,
                                                                  JsonConvert.SerializeObject(payload, jss),
                                                                  "App",
                                                                  "Resend");


                    }
                }
            }
            catch (Exception ex)
            {
                string log = "Send 160 error - " + ex.Message + "\n" + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
                res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);
                retval = -1;
            }

        }

        public void Send170Event(string url,
                        int interfaceId,
                        string interfaceUniqueId,
                        string orderUniqueId,
                        string userName,
                        string password,
                        int accountID,
                        DateTime eventDate,
                       string note)
        {
            int retval = 0;
            Task<int> res;
            try
            {
                string log = "Send 170 - " + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
                res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);

                HttpResponse httpResponse = new HttpResponse();
                Event120_200 avsRequest = new Event120_200();
                avsRequest.requestDateTime = System.DateTime.Now;
                avsRequest.internalAccountIdentifier = interfaceUniqueId;
                avsRequest.accountID = accountID;
                avsRequest.loginAccountIdentifier = userName;
                avsRequest.loginAccountPassword = password;
                avsRequest.requestData = new List<RequestData>();
                avsRequest.requestData.Add(new RequestData());
                avsRequest.requestData[0].eventDate = eventDate;
                avsRequest.requestData[0].comment = "";
                avsRequest.requestData[0].description = "";
                avsRequest.requestData[0].propertyRequest = new List<PropertyRequest>();
                avsRequest.requestData[0].propertyRequest.Add(new PropertyRequest());
                avsRequest.requestData[0].propertyRequest[0].eventCode = 170;
                avsRequest.requestData[0].propertyRequest[0].uniqueID = new Guid(orderUniqueId);
                avsRequest.requestData[0].propertyRequest[0].note = note;
                var payload = new { request = avsRequest };
                JsonSerializerSettings jss = new JsonSerializerSettings();
                jss.NullValueHandling = NullValueHandling.Ignore;
                httpResponse = _httpService.PostMessage(url,
                                         JsonConvert.SerializeObject(payload, jss),
                                         "",
                                         "");
                if (httpResponse.success == true)
                {
                    //check status code
                    JToken avsResponse = JToken.Parse(httpResponse.responseData);
                    int.TryParse(avsResponse["response"]["responseData"][0]["statusCode"].ToString(), out retval);
                    if (retval == 0)
                    {
                        retval = 1;
                    }
                    else
                    {
                        res = _commonRepository.SaveInterfaceData(interfaceId,
                                                                  JsonConvert.SerializeObject(payload, jss),
                                                                  "App",
                                                                  "Resend");


                    }
                }
            }
            catch (Exception ex)
            {
                string log = "Send 170 error - " + ex.Message + "\n" + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
                res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);
                retval = -1;
            }

        }

        public void Send180Event(string url,
                       int interfaceId,
                       string interfaceUniqueId,
                       string orderUniqueId,
                       string userName,
                       string password,
                        int accountID,
                        DateTime eventDate,
                      string note)
        {
            int retval = 0;
            Task<int> res;
            try
            {
                string log = "Send 180 - " + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
                res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);

                HttpResponse httpResponse = new HttpResponse();
                Event120_200 avsRequest = new Event120_200();
                avsRequest.requestDateTime = System.DateTime.Now;
                avsRequest.internalAccountIdentifier = interfaceUniqueId;
                avsRequest.accountID = accountID;
                avsRequest.loginAccountIdentifier = userName;
                avsRequest.loginAccountPassword = password;
                avsRequest.requestData = new List<RequestData>();
                avsRequest.requestData.Add(new RequestData());
                avsRequest.requestData[0].eventDate = eventDate;
                avsRequest.requestData[0].comment = "";
                avsRequest.requestData[0].description = "";
                avsRequest.requestData[0].propertyRequest = new List<PropertyRequest>();
                avsRequest.requestData[0].propertyRequest.Add(new PropertyRequest());
                avsRequest.requestData[0].propertyRequest[0].eventCode = 180;
                avsRequest.requestData[0].propertyRequest[0].uniqueID = new Guid(orderUniqueId);
                avsRequest.requestData[0].propertyRequest[0].note = note;
                var payload = new { request = avsRequest };
                JsonSerializerSettings jss = new JsonSerializerSettings();
                jss.NullValueHandling = NullValueHandling.Ignore;
                httpResponse = _httpService.PostMessage(url,
                                         JsonConvert.SerializeObject(payload, jss),
                                         "",
                                         "");
                if (httpResponse.success == true)
                {
                    //check status code
                    JToken avsResponse = JToken.Parse(httpResponse.responseData);
                    int.TryParse(avsResponse["response"]["responseData"][0]["statusCode"].ToString(), out retval);
                    if (retval == 0)
                    {
                        retval = 1;
                    }
                    else
                    {
                        res = _commonRepository.SaveInterfaceData(interfaceId,
                                                                  JsonConvert.SerializeObject(payload, jss),
                                                                  "App",
                                                                  "Resend");


                    }
                }
            }
            catch (Exception ex)
            {
                string log = "Send 180 error - " + ex.Message + "\n" + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
                res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);
                retval = -1;
            }

        }



        public void SendAsyncResponse(string url,
                                      int interfaceId,
                                      string interfaceUniqueId,
                                      string orderUniqueId,
                                      string userName,
                                      string password,
                                      int accountID,
                                      DateTime eventDate,
                                      string comment,
                                      int eventCode,
                                      int statusCode)
        {
            AsyncResponse response = new AsyncResponse();
            ResponseData responseData = new ResponseData();
            PropertyResponse propertyResponse = new PropertyResponse();
            string log = "Send Async - " + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
            Task<int> res;
            try { 
            res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);

            HttpResponse httpResponse = new HttpResponse();

            response.responseDateTime = System.DateTime.Now;
            response.responseData = new List<ResponseData>();
            responseData.description = "";
            response.internalAccountIdentifier = interfaceUniqueId;
            responseData.comment = comment;
            responseData.statusCode = statusCode;
            response.responseData.Add(responseData);
            response.responseData[0].propertyResponse = new List<PropertyResponse>();
            response.responseData[0].propertyResponse.Add(new PropertyResponse());
            response.responseData[0].propertyResponse[0].eventCode = eventCode;
            response.responseData[0].propertyResponse[0].uniqueID = new Guid(orderUniqueId);
                response.responseData[0].propertyResponse[0].customerFee = null;

                var payload = new { response = response };
            JsonSerializerSettings jss = new JsonSerializerSettings();
            jss.NullValueHandling = NullValueHandling.Ignore;
            httpResponse = _httpService.PostMessage(url,
                                     JsonConvert.SerializeObject(payload, jss),
                                     "",
                                     "");
            if (httpResponse.success == true)
            {
                //check status code
                JToken avsResponse = JToken.Parse(httpResponse.responseData);
                int retval = 0;
                int.TryParse(avsResponse["response"]["responseData"][0]["statusCode"].ToString(), out retval);
                if (retval != 0)
                {
                    res = _commonRepository.SaveInterfaceData(interfaceId,
                                                              JsonConvert.SerializeObject(payload, jss),
                                                              "App",
                                                              "Resend");


                }
            }
        }
            catch (Exception ex)
            {
                log = "Send Async error - " + ex.Message + "\n" + ", " + interfaceUniqueId + ", " + userName + ", " + password + ", ";
                res = _logService.SaveLogEntry(interfaceId, log, interfaceUniqueId);
            }



        }



        #endregion
    }
}
