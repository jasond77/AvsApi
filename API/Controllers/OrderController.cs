using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceInterface;

namespace API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        #region[Dependencies]

        private IOrderService _orderService;
        private IMessageService _messageService;
        private ILogService _logService;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly string _currentUser;
        readonly IConfiguration _configuration;

        public OrderController(IOrderService orderService,
                                ILogService logService,
                                IMessageService messageService,
                                IHttpContextAccessor httpContextAccessor,
                                IConfiguration configuration)
        {
            _orderService = orderService;
            _logService = logService;
            _messageService = messageService;
            _httpContextAccessor = httpContextAccessor;
            _currentUser = httpContextAccessor.HttpContext.User.Identity.Name;
            _configuration = configuration;
        }
        #endregion




        // GET api/values
        [HttpGet("~/api/100/")]
        public async Task<IActionResult> Get100Event()
        {
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
            // string s = await _orderService.GetOrder(1);
            return Ok(new { request = request });
        }

        [HttpPost("~/api/100/")]
        public IActionResult Post100Event([FromBody] string payload)
        {

            //parse json
            Response response = new Response();
            ResponseData responseData = new ResponseData();
            response.responseDateTime = System.DateTime.Now;
            responseData.statusCode = 0;
            JObject json = JObject.Parse(payload);
            if(json == null)
            {
                responseData.statusCode = -1;
            }

            response.responseData = new List<ResponseData>();

            return Ok(new { response = response });

        }

        [HttpGet("~/api/asyncResponse/")]
        public async Task<IActionResult> GetAsyncResponse()
        {
            AsyncResponse response = new AsyncResponse();
            ResponseData responseData = new ResponseData();

            response.responseDateTime = System.DateTime.Now;
            response.responseData = new List<ResponseData>();
            responseData.comment = "comment about response (optional)";
            responseData.description = "description (optional)";
            responseData.statusCode = 0;
            response.responseData.Add(responseData);
            // string s = await _orderService.GetOrder(1);
            return Ok(new { response = response });
        }

        [HttpGet("~/api/110/")]
        public async Task<IActionResult> Get110Event()
        {
            Event110 request = new Event110();
            ResponseData responseData = new ResponseData();
            PropertyResponse propertyResponse= new PropertyResponse();
            request.responseData = new List<ResponseData>();

            responseData.comment = "";
            responseData.description = "";

            propertyResponse.uniqueID = new Guid();
            propertyResponse.eventCode = 115;
            responseData.propertyResponse = new List<PropertyResponse>();
            responseData.propertyResponse.Add(propertyResponse);
            request.responseData.Add(responseData);

            return Ok(new { request = request });
        }

        [HttpGet("~/api/120-200/")]
        public async Task<IActionResult> Get120_200Event()
        {
            Event120_200 request = new Event120_200();
            RequestData requestData = new RequestData();
            PropertyRequest propertyRequest = new PropertyRequest();
            request.requestData = new List<RequestData>();

            requestData.comment = "";
            requestData.eventDate = System.DateTime.Now;
            requestData.description = "";

            propertyRequest.uniqueID = new Guid();
            propertyRequest.eventCode = 120;

            requestData.propertyRequest = new List<PropertyRequest>();
            requestData.propertyRequest.Add(propertyRequest);
            requestData.propertyRequest[0].note = "order note";
            request.requestData.Add(requestData);

            return Ok(new { request = request });
        }

        [HttpGet("~/api/130/")]
        public async Task<IActionResult> Get130Event()
        {
            Event120_200 request = new Event120_200();
            RequestData requestData = new RequestData();
            PropertyRequest propertyRequest = new PropertyRequest();
            File file = new File();
            request.requestData = new List<RequestData>();



            requestData.comment = "upload a new document";
            requestData.description = "";
            requestData.eventDate = System.DateTime.Now;

            propertyRequest.uniqueID = new Guid();
            propertyRequest.eventCode = 130;

            file.document = "http://www.applied-valuation.com/file.tif";
            file.encodingType = "Url";
            file.extension = "tif";
            file.name = "file";
            file.type = "Image";
            propertyRequest.files = new List<File>();
            propertyRequest.files.Add(file);


            requestData.propertyRequest = new List<PropertyRequest>();
            requestData.propertyRequest.Add(propertyRequest);
            request.requestData.Add(requestData);

            return Ok(new { request = request });
        }

        [HttpGet("~/api/140/")]
        public async Task<IActionResult> Get140Event()
        {
            Event120_200 request = new Event120_200();
            RequestData requestData = new RequestData();
            PropertyRequest propertyRequest = new PropertyRequest();
            File file = new File();
            request.requestData = new List<RequestData>();



            requestData.comment = "upload a new document";
            requestData.description = "";
            requestData.eventDate = System.DateTime.Now;

            propertyRequest.uniqueID = new Guid();
            propertyRequest.eventCode = 140;

            file.document = "http://www.applied-valuation.com/file.tif";
            file.encodingType = "Url";
            file.extension = "tif";
            file.name = "file";
            file.type = "Image";
            propertyRequest.files = new List<File>();
            propertyRequest.files.Add(file);


            requestData.propertyRequest = new List<PropertyRequest>();
            requestData.propertyRequest.Add(propertyRequest);
            request.requestData.Add(requestData);

            return Ok(new { request = request });
        }

        [HttpPost("~/api/orders")]
        public async Task<IActionResult> UpdateOrders([FromBody] object payload)
        {
            Response response = new Response();
            ResponseData responseData = new ResponseData();
            response.responseDateTime = System.DateTime.Now;
            response.responseData = new List<ResponseData>();
            response.responseData.Add(new ResponseData());
            try
            {

                //log payload

                JObject avsRequest = JObject.Parse(JsonConvert.SerializeObject(payload));
                int s = await _logService.SaveLogEntry(1, avsRequest.ToString(), "");

                //parse payload
                if (avsRequest["request"]["loginAccountIdentifier"] == null)
                {
                    response.responseData[0].statusCode = -1;
                    response.responseData[0].comment = "An error occurred. No username found.";
                }
                else if (avsRequest["request"]["loginAccountPassword"] == null)
                {
                    response.responseData[0].statusCode = -1;
                    response.responseData[0].comment = "An error occurred. No password found.";
                }
                else if (avsRequest["request"]["requestData"] == null)
                {
                    response.responseData[0].statusCode = -1;
                    response.responseData[0].comment = "An error occurred. No requestData found.";
                 }
               else if (avsRequest["request"]["requestData"][0]["propertyRequest"] == null)
                {
                    response.responseData[0].statusCode = -1;
                    response.responseData[0].comment = "An error occurred. No requestData found.";
                }
                else
                {
                    //save payload
                    string ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                    if (_httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].ToString() != "" )
                    {
                        ip = _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"][0];
                        Console.WriteLine(ip + "|X-Forwarded-For");
                    }
                    else if (_httpContextAccessor.HttpContext.Request.Headers["REMOTE_ADDR"].ToString() != "")
                    {
                        ip = _httpContextAccessor.HttpContext.Request.Headers["REMOTE_ADDR"][0];
                    }
                    int interfaceDataId = await _logService.SaveInterfaceData(1, avsRequest.ToString(), ip, avsRequest["request"]["requestData"][0]["propertyRequest"][0]["eventCode"].ToString());


                    Thread thread = new Thread(() => _orderService.ProcessInterfaceData(interfaceDataId));
                    thread.Start();


                }

            }
            catch (Exception ex)
            {
                response.responseData[0].statusCode = -1;
                response.responseData[0].comment = "An error occurred, unable to parse data." + ex.Message;
                if(response.responseData[0].comment.Length > 500)
                {
                    response.responseData[0].comment.Substring(0, 500);
                }
                response.responseData[0].description = _httpContextAccessor.HttpContext.Request.QueryString + "\n"
                    + _httpContextAccessor.HttpContext.Request.Headers.ToString()
                    + _httpContextAccessor.HttpContext.Request.Host.Host.ToString();
            }






            return Ok(new { response = response });
        }

        [HttpPost("~/api/post/")]
        public IActionResult postData([FromBody] object payload)
        {
            string json = JsonConvert.SerializeObject(payload);
            System.IO.FileStream fs = System.IO.File.Create("c:\\temp\\avs\\message.json");
            Byte[] info = System.Text.Encoding.Default.GetBytes(json);
            fs.Write(info, 0, info.Length);
            fs.Close();
            JObject avsRequest = JObject.Parse(json);

            return Ok(new { response = avsRequest });


        }

        [HttpGet("~/api/send/")]
        public IActionResult sendData([FromQuery] string code)
        {
            string interfaceUniqueId = "619299328";
            string orderUniqueId = "0dbf75ea-cd31-477f-913b-fd6eb1fdb62f";
            string url = "http://localhost:55777/api/post";
            List<File> files;
            switch (code) {

                case "120":
                    _messageService.Send120Event(url, 1, interfaceUniqueId, orderUniqueId, "", "", 809, DateTime.Now,"test note");
                    break;

                case "130":
                    files = new List<File>();
                    File f = new File();
                    f.type = "Image";
                    f.extension = "pdf";
                    f.document = "";
                    f.name = "image.pdf";
                    files.Add(f);
                    _messageService.Send130Event(url, 1, interfaceUniqueId, orderUniqueId, "", "", 809, DateTime.Now, files);
                    break;

                case "140":
                    files = new List<File>();
                    f = new File();
                    f.type = "Invoice";
                    f.extension = "pdf";
                    f.document = "";
                    f.name = "invoice.pdf";
                    files.Add(f);
                    f = new File();
                    f.type = "Report";
                    f.extension = "pdf";
                    f.document = "";
                    f.name = "appraisal.pdf";
                    files.Add(f);
                    _messageService.Send140Event(url, 1, interfaceUniqueId, orderUniqueId, "", "", 809, DateTime.Now, files);
                    break;

                case "150":
                    _messageService.Send150Event(url, 1, interfaceUniqueId, orderUniqueId, "", "", 809, DateTime.Now, "Order has been placed on hold.");
                    break;

                case "160":
                    _messageService.Send160Event(url, 1, interfaceUniqueId, orderUniqueId, "", "", 809, DateTime.Now, "Order has been taken off hold.");
                    break;

                case "170":
                    _messageService.Send170Event(url, 1, interfaceUniqueId, orderUniqueId, "", "", 809, DateTime.Now, "Order is cancelled.");
                    break;

                case "180":
                    _messageService.Send180Event(url, 1, interfaceUniqueId, orderUniqueId, "", "", 809, DateTime.Now, "Order can resume.");
                    break;


            }
            return Ok(new { response = "" });


        }


        [HttpGet("~/api/process/")]
        public IActionResult processData()
        {

                    _orderService.ProcessInterfaceData();


            return Ok();


        }
    }
}
