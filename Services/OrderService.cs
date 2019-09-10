using DataContracts;
using DataInterface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Services
{
    public class OrderService : IOrderService
    {
        #region[Dependencies]

        //private readonly IConfiguration _config;
        private ICommonRepository _commonRepository;
        private IHttpService _httpService;
        private ILogService _logService;
        private IMessageService _messageService;
        private IOrderRepository _orderRepository;
        public OrderService(IOrderRepository orderRepository,
                            IHttpService httpService,
                            ILogService logService,
                            ICommonRepository commonRepository,
                            IMessageService messageService)
        {
            _orderRepository = orderRepository;
            _logService = logService;
            _commonRepository = commonRepository;
            _httpService = httpService;
            _messageService = messageService;
        }


        #endregion

        #region[Public Methods]


        public async Task<string> GetOrder(int orderId)
        {
            string s = "";
            try
            {
                s = await _orderRepository.GetOrderByID(orderId);
            }
            catch (Exception ex)
            {

            }
            return s;

        }


        public async void ProcessInterfaceData(int? interfaceDataId = null)
        {
            List<Dictionary<string, object>> interfaceData = new List<Dictionary<string, object>>();
            try
            {
                interfaceData = await _commonRepository.GetInterfaceDataNotProcessed();
                //loop through list
                foreach (var item in interfaceData)
                {
                    if (interfaceDataId == null || interfaceDataId == int.Parse(item["business_interface_data_id"].ToString()))
                    {
                        string message = item["txt_post_data"].ToString();
                        string messageType = item["txt_message_type"].ToString();
                        int interfaceId = int.Parse(item["business_interface_id"].ToString());
                        int businessId = int.Parse(item["business_id"].ToString());
                        interfaceDataId = item["business_interface_data_id"] == null ? 0 : int.Parse(item["business_interface_data_id"].ToString());
                        
                        ProcessMessage(message, messageType, interfaceId, int.Parse(interfaceDataId.ToString()), businessId);
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }


        public async void ProcessMessage(string message,
                                         string messageType,
                                         int interfaceId,
                                         int interfaceDataId,
                                         int businessId)
        {

            try
            {
                JToken avsRequest = JToken.Parse(message);
                JToken avsData;
                int customerAccountId = int.Parse(avsRequest["request"]["accountID"].ToString());
                int processed = 0;
                string json;
                int ret;
                int orderDetailId = 0;
                string interfaceUniqueId = avsRequest["request"]["internalAccountIdentifier"].ToString();
                string orderUniqueId = "";
                string url = "";
                string username = "";
                string password = "";
                string log;

                json = await _commonRepository.GetInterfaceData(interfaceId);
                if (json != "")
                {
                    avsData = JToken.Parse(json);
                    url = avsData["post_url"].ToString();
                    username = avsData["txt_interface_username"].ToString();
                    password = avsData["txt_interface_password"].ToString();
                }

                foreach (JToken pr in avsRequest["request"]["requestData"])
                {



                    if(pr["propertyRequest"][0]["uniqueID"] != null)
                    {
                        orderUniqueId = pr["propertyRequest"][0]["uniqueID"].ToString();
                       json = await _orderRepository.GetOrderByUniqueId(orderUniqueId);
                        if (json != "")
                        {
                            avsData = JToken.Parse(json);
                            int.TryParse(avsData["order_detail_id"].ToString(), out orderDetailId);

                        }
                    }

                    log = "Process Message - " + messageType + "\n" + message;
                    ret = await _logService.SaveLogEntry(interfaceId, log, interfaceDataId.ToString());

                    switch (pr["propertyRequest"][0]["eventCode"].ToString())
                    {
                        case "100":
                            //create new order
                            CreateOrder(pr["propertyRequest"][0].ToString(),
                                        interfaceId,
                                        customerAccountId,
                                        interfaceUniqueId,
                                        interfaceDataId,
                                        businessId);


                            break;

                        case "120":
                                //save comment
                                ret = await  _orderRepository.SaveOrderNote(orderDetailId,
                                                                             pr["propertyRequest"][0]["note"].ToString(),
                                                                             "admin",
                                                                             0,
                                                                             "Interface",
                                                                             "customer",
                                                                             8);
                                if (ret != 0)
                                {
                                    //send async response
                                    _messageService.SendAsyncResponse(url,
                                                                        interfaceId,
                                                                        interfaceUniqueId,
                                                                        orderUniqueId,
                                                                        username,
                                                                        password,
                                                                        customerAccountId,
                                                                        DateTime.Now,
                                                                        "Comment message",
                                                                        120,
                                                                        0);

                                    ret = await _commonRepository.UpdateInterfaceDataProcessed(interfaceDataId);
                               }
                                
                                 break;

                        case "130":


                                ret = await SaveFiles(orderDetailId, pr["propertyRequest"][0]["files"]);

                                if (ret != 0)
                                {
                                    //send async response
                                    _messageService.SendAsyncResponse(url,
                                                                        interfaceId,
                                                                        interfaceUniqueId,
                                                                        orderUniqueId,
                                                                        username,
                                                                        password,
                                                                        customerAccountId,
                                                                        DateTime.Now,
                                                                        "File upload message",
                                                                        130,
                                                                        0);
                                    ret = await _commonRepository.UpdateInterfaceDataProcessed(interfaceDataId);

                                }

                            break;

                        case "150":

                                ret = await _orderRepository.SaveOrderNote(orderDetailId,
                                             "Order placed on hold by client: " + pr["propertyRequest"][0]["note"].ToString(),
                                             "admin",
                                             0,
                                             "Interface",
                                             "customer",
                                             8);
                                //ret = await _orderRepository.SaveOrderDetailEvent(orderDetailId, 6, 8);
                                if (ret != 0)
                                {
                                    //send async response
                                    _messageService.SendAsyncResponse(url,
                                                                        interfaceId,
                                                                        interfaceUniqueId,
                                                                        orderUniqueId,
                                                                        username,
                                                                        password,
                                                                        customerAccountId,
                                                                        DateTime.Now,
                                                                        "On hold message",
                                                                        150,
                                                                        0);
                                    ret = await _commonRepository.UpdateInterfaceDataProcessed(interfaceDataId);
                                }
                            break;
                        case "160":

                            ret = await _orderRepository.SaveOrderNote(orderDetailId,
                                             "Order taken off hold by client: " + pr["propertyRequest"][0]["note"].ToString(),
                                             "admin",
                                             0,
                                             "Interface",
                                             "customer",
                                             8);
                                //ret = await _orderRepository.SaveOrderDetailEvent(orderDetailId, 6, 8);
                                if (ret != 0)
                                {
                                    //send async response
                                    _messageService.SendAsyncResponse(url,
                                                                    interfaceId,
                                                                    interfaceUniqueId,
                                                                    orderUniqueId,
                                                                    username,
                                                                    password,
                                                                    customerAccountId,
                                                                    DateTime.Now,
                                                                    "Order taken off hold message",
                                                                    160,
                                                                    0);
                                    ret = await _commonRepository.UpdateInterfaceDataProcessed(interfaceDataId);
                                }

                            break;

                        case "170":

                            ret = await _orderRepository.SaveOrderNote(orderDetailId,
                                             "Order cancelled by client: " + pr["propertyRequest"][0]["note"].ToString(),
                                             "admin",
                                             0,
                                             "Interface",
                                             "customer",
                                             8);
                                //ret = await _orderRepository.SaveOrderDetailEvent(orderDetailId, 6, 8);
                                if (ret != 0)
                             {
                                   //send async response
                                    _messageService.SendAsyncResponse(url,
                                                                    interfaceId,
                                                                        interfaceUniqueId,
                                                                    orderUniqueId,
                                                                    username,
                                                                    password,
                                                                    customerAccountId,
                                                                    DateTime.Now,
                                                                    "Order canncelled message",
                                                                    170,
                                                                    0);
                                ret = await _commonRepository.UpdateInterfaceDataProcessed(interfaceDataId);
                                }

                            break;
                        case "180":

                            ret = await _orderRepository.SaveOrderNote(orderDetailId,
                                             "Order to resume/reactivate by client: " + pr["propertyRequest"][0]["note"].ToString(),
                                             "admin",
                                             0,
                                             "Interface",
                                             "customer",
                                             8);
                                //ret = await _orderRepository.SaveOrderDetailEvent(orderDetailId, 6, 8);
                                if (ret != 0)
                                {
                                    //send async response
                                    _messageService.SendAsyncResponse(url,
                                                                    interfaceId,
                                                                        interfaceUniqueId,
                                                                    orderUniqueId,
                                                                    username,
                                                                    password,
                                                                    customerAccountId,
                                                                    DateTime.Now,
                                                                    "Resume order message",
                                                                    180,
                                                                    0);
                                    ret = await _commonRepository.UpdateInterfaceDataProcessed(interfaceDataId);
                                }
                            break;

                        case "190":

                            ret = await _orderRepository.SaveOrderNote(orderDetailId,
                                             "Fee change by client: " + pr["propertyRequest"][0]["note"].ToString(),
                                             "admin",
                                             0,
                                             "Interface",
                                             "customer",
                                             8);
                                //ret = await _orderRepository.SaveOrderDetailEvent(orderDetailId, 6, 8);
                                if (ret != 0)
                                {
                                    //send async response
                                    _messageService.SendAsyncResponse(url,
                                                                    interfaceId,
                                                                        interfaceUniqueId,
                                                                    orderUniqueId,
                                                                    username,
                                                                    password,
                                                                    customerAccountId,
                                                                    DateTime.Now,
                                                                    "Fee change message",
                                                                    130,
                                                                    0);
                                    ret = await _commonRepository.UpdateInterfaceDataProcessed(interfaceDataId);
                                }
                            break;

                    }

                }

            }
            catch (Exception ex)
            {

            }

        }


        public async void CreateOrder(string message,
                                           int interfaceId,
                                           int customerAccountId,
                                           string interfaceUniqueId,
                                           int interfaceDataId,
                                           int businessId)
        {

            int orderDetailId = 0;
            int orderId = 0;
            int propertyId = 0;
            int customerProductId = 0;
            int stateId = 0;
            string county = "";
            decimal loanAmount = 0;
            decimal appraisedValue = 0;
            Response avsResponse = new Response();
            avsResponse.responseData = new List<ResponseData>();
            avsResponse.responseData.Add(new ResponseData());
            avsResponse.responseData[0].propertyResponse = new List<PropertyResponse>();
            avsResponse.responseData[0].propertyResponse.Add(new PropertyResponse());
            string json;
            JToken avsData;
            int proceed = 1;
            try
            {
                string log = "CreateOrder - " + message;
                int res = await _logService.SaveLogEntry(interfaceId, log, interfaceDataId.ToString());

                JToken avsRequest = JToken.Parse(message);
                //log = "Customer lookup  " + customer_account_id + " " + customer_account_id + vbNewLine
                //log += "XMLData (Received from PS Interface): " + vbNewLine
                //log += XMLData + vbNewLine
                //Common.Write_To_Log(log, Business_Interface_ID, txt_Interface_Unique_ID)


                //  string json =  await _commonRepository.GetInterfaceLookup(interfaceId, "CLIENTID", customer_account_id.ToString(), 0);
                //  JToken avsData = JToken.Parse(json);


                //log = "Message Type: " + message_type.ToUpper + vbNewLine
                //log += "Customer lookup failed " + customer_account_id + " " + retval(0).ToString + vbNewLine
                //log += "XMLData (Received from PS Interface): " + vbNewLine
                //log += XMLData + vbNewLine
                //Common.Write_To_Log(log, Business_Interface_ID, txt_Interface_Unique_ID)


                json = await _commonRepository.GetInterfaceLookup(interfaceId,
                                                                         "ProductName",
                                                                         avsRequest["products"][0]["name"].ToString(),
                                                                         customerAccountId);
                if (json == "")
                {
                    log = "Message Type: 100\n"
                        + "Interface Data ID = " + interfaceDataId.ToString() + "\n"
                        + "No Product Found ProductName = " + avsRequest["products"][0]["name"].ToString() + "\n";
                    res = await _logService.SaveLogEntry(interfaceId, log, interfaceDataId.ToString());
                    avsResponse.responseData[0].propertyResponse[0].eventCode = 115;
                    avsResponse.responseData[0].description = log;
                    avsResponse.responseData[0].comment = "No Product mapped to ProductName = " + avsRequest["products"][0]["name"].ToString();
                    proceed = 0;
                }
                else
                {
                    avsData = JToken.Parse(json);
                    customerProductId = int.Parse(avsData["txt_internal_field_value"].ToString());
                }



                json = await _orderRepository.GetOrderByInterfaceId(interfaceId, interfaceUniqueId);
                if (json != "")
                {
                    avsData = JToken.Parse(json);
                    log = "Message Type: 100\n"
                        + "Customer Order ID = " + interfaceDataId.ToString() + "\n"
                        + "Existing Order Unique ID = " + avsData["unique_id"].ToString() + "\n";
                    res = await _logService.SaveLogEntry(interfaceId, log, interfaceDataId.ToString());
                    avsResponse.responseData[0].propertyResponse[0].eventCode = 115;
                    avsResponse.responseData[0].description = log;
                    avsResponse.responseData[0].comment = "Existing Order Unique ID = " + avsData["unique_id"].ToString();
                    proceed = 0;
                }

                json = await _commonRepository.GetZipInfo(avsRequest["subjectAddress"]["postalCode"].ToString());
                if (json == "")
                {
                    log = "Message Type: 100\n"
                        + "Interface Data ID = " + interfaceDataId.ToString() + "\n"
                        + "Zip not Found Zip = " + avsRequest["subjectAddress"]["postalCode"].ToString() + "\n";
                    res = await _logService.SaveLogEntry(interfaceId, log, interfaceDataId.ToString());
                    //return 0;
                }
                else
                {
                    avsData = JToken.Parse(json);
                    county = avsData["txt_county_name"].ToString();
                    stateId = int.Parse(avsData["state_id"].ToString());

                }



                if (orderDetailId == 0 && proceed == 1)
                {
                    propertyId = await _orderRepository.SaveProperty(0,
                                                           avsRequest["subjectAddress"]["streetAddress"].ToString(),
                                                           "",
                                                           avsRequest["subjectAddress"]["city"].ToString(),
                                                           stateId,
                                                           avsRequest["subjectAddress"]["postalCode"].ToString(),
                                                           county, //avsRequest["subjectAddress"]["county"].ToString(),
                                                           8);
                    if (propertyId == 0)
                    {
                        log = "Message Type: 100\n"
                            + "Interface Data ID = " + interfaceDataId.ToString() + "\n"
                            + "Property Not Saved = "
                            + avsRequest["subjectAddress"]["streetAddress"].ToString() + ", "
                            + avsRequest["subjectAddress"]["city"].ToString() + ", "
                            + avsRequest["subjectAddress"]["county"].ToString() + ", "
                            + county  + ", "
                            + "\n";
                        res = await _logService.SaveLogEntry(interfaceId, log, interfaceDataId.ToString());
                        avsResponse.responseData[0].propertyResponse[0].eventCode = 115;
                        avsResponse.responseData[0].description = log;
                        avsResponse.responseData[0].comment = "Error saving property address ";
                        proceed = 0;
                    }

                    if (proceed == 1)
                    {

                        decimal.TryParse(avsRequest["loanInfo"]["loanAmount"].ToString(), out loanAmount);
                        orderId = await _orderRepository.SaveOrder(0,
                                                                propertyId,
                                                                customerAccountId,
                                                                avsRequest["loanInfo"]["lenderCaseIdentifier"].ToString(),
                                                                loanAmount,
                                                                "",
                                                                0,
                                                                businessId,
                                                                8);
                        if (orderId == 0)
                        {
                            log = "Message Type: 100\n"
                                + "Interface Data ID = " + interfaceDataId.ToString() + "\n"
                                + "Order Not Saved = "
                                + propertyId.ToString() + ", "
                                + customerAccountId.ToString() + ", "
                                + avsRequest["loanInfo"]["lenderCaseIdentifier"].ToString() + ", "
                                + loanAmount
                                + "\n";
                            res = await _logService.SaveLogEntry(interfaceId, log, interfaceDataId.ToString());
                            avsResponse.responseData[0].propertyResponse[0].eventCode = 115;
                            avsResponse.responseData[0].description = log;
                            avsResponse.responseData[0].comment = "Error saving order.";
                            proceed = 0;
                        }

                    }

                    decimal fee = 0;
                    fee = await _orderRepository.GetCustomerFee(customerProductId, stateId, county);
                    if (fee == 0)
                    {
                        log = "Message Type: 100\n"
                            + "Interface Data ID = " + interfaceDataId.ToString() + "\n"
                            + "Fee not Found Product = " + customerProductId + ", " + stateId + ", " + county + "\n";
                        res = await _logService.SaveLogEntry(interfaceId, log, interfaceDataId.ToString());
                    }

                    if (proceed == 1)
                    {
                        decimal.TryParse(avsRequest["loanInfo"]["appraisedValue"].ToString(), out appraisedValue);
                        orderDetailId = await _orderRepository.SaveOrderDetail(0,
                                                                orderId,
                                                                "2,year||2,month||5,id||",
                                                                customerProductId,
                                                                0,
                                                                fee, //0,
                                                                System.DateTime.Now.AddDays(7),
                                                                System.DateTime.Now.AddDays(14),
                                                                null,
                                                                "",
                                                                0,
                                                                0,
                                                                System.DateTime.Now,
                                                                interfaceUniqueId,
                                                                "",
                                                                "",
                                                                "AVS API",
                                                                "",
                                                                appraisedValue,
                                                                "",
                                                                8);

                        if (orderDetailId == 0)
                        {
                            log = "Message Type: 100\n"
                                + "Interface Data ID = " + interfaceDataId.ToString() + "\n"
                                + "OrderDetail Not Saved = "
                                + propertyId.ToString() + ", "
                                + customerAccountId.ToString() + ", "
                                + avsRequest["loanInfo"]["lenderCaseIdentifier"].ToString() + ", "
                                + avsRequest["loanInfo"]["loanAmount"].ToString()
                                + "\n";
                            res = await _logService.SaveLogEntry(interfaceId, log, interfaceDataId.ToString());
                            avsResponse.responseData[0].propertyResponse[0].eventCode = 115;
                            avsResponse.responseData[0].description = log;
                            avsResponse.responseData[0].comment = "Error saving order detail.";
                            proceed = 0;
                        }

                    }

                    if (proceed == 1)
                    {

                        res = await _logService.SaveLogEntry(interfaceId, "Order Created - " + orderDetailId.ToString(), interfaceDataId.ToString());

                        //save note
                        res = await _orderRepository.SaveOrderNote(orderDetailId,
                                                                    "",
                                                                    "admin",
                                                                    0,
                                                                    "Interface",
                                                                    "admin",
                                                                    8);

                        //save tracking
                        DateTime d = DateTime.Parse("1/1/1900");
                        res = await _orderRepository.SaveInterfaceTracking(orderDetailId,
                                                interfaceUniqueId,
                                                DateTime.Now,
                                                d,
                                                false,
                                                fee,
                                                d,
                                                false,
                                                false,
                                                interfaceId,
                                                "",
                                                "");
                        avsResponse.responseData[0].propertyResponse[0].eventCode = 110;

                        //save contacts
                        //foreach (var contact in avsRequest["contacts"])
                        //{

                        //    res = await _orderRepository.SaveOrderContact(orderDetailId,
                        //                                                  0,
                        //                                                  contact["firstName"].ToString(),
                        //                                                  contact["lastName"].ToString(),
                        //                                                  contact["middleName"].ToString(),
                        //                                                  contact["telephoneNumber1"].ToString(),
                        //                                                  contact["telephoneNumber2"].ToString(),
                        //                                                  "",
                        //                                                  "",
                        //                                                  "",
                        //                                                  0,
                        //                                                  8,
                        //                                                  DateTime.Now);

                        //}

                        FileStream fs;
                        string orderFolder = await _orderRepository.GetOrderFolder(orderDetailId);
                        if (Directory.Exists(orderFolder) == true)
                        {
                            fs = System.IO.File.Create(orderFolder + "interfaceData.json");
                            Byte[] info = System.Text.Encoding.Default.GetBytes(avsRequest.ToString());
                            fs.Write(info, 0, info.Length);
                            fs.Close();
                        }

                        SaveFiles(orderDetailId, avsRequest["files"]);

                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (avsResponse.responseData[0].propertyResponse[0].eventCode == 110)
            {
                json = await _orderRepository.GetOrderByInterfaceId(interfaceId, interfaceUniqueId);
                if (json != "")
                {
                    avsData = JToken.Parse(json);
                    decimal fee = 0;
                    decimal.TryParse(avsData["mny_customer_fee"].ToString(), out fee);
                    _messageService.Send110Event(avsData["post_url"].ToString(),
                                                         interfaceId,
                                                         avsData["txt_interface_unique_id"].ToString(),
                                                         avsData["unique_id"].ToString(),
                                                         avsData["txt_interface_username"].ToString(),
                                                         avsData["txt_interface_password"].ToString(),
                                                         fee);
                }
            }
            else
            {
                json = await _commonRepository.GetInterfaceData(interfaceId);
                if (json != "")
                {
                    avsData = JToken.Parse(json);
                    _messageService.Send115Event(avsData["post_url"].ToString(),
                                         interfaceId,
                                         interfaceUniqueId,
                                         avsData["txt_interface_username"].ToString(),
                                         avsData["txt_interface_password"].ToString(),
                                         avsResponse.responseData[0].comment,
                                         avsResponse.responseData[0].description);

                }
            }



        }


        public async Task<int> SaveFiles(int orderDetailId,
                              JToken fileCollection)
        {
                   int ret = 0;
         try
            {
                FileStream fs;
                string orderFolder = await _orderRepository.GetOrderFolder(orderDetailId);
                foreach (var file in fileCollection)
                {
                    fs = System.IO.File.Create(orderFolder + file["name"].ToString());
                    Byte[] info = System.Convert.FromBase64String(file["document"].ToString());
                    fs.Write(info, 0, info.Length);
                    fs.Close();

                }
                ret = 1;

            }
            catch(Exception ex)
            {


            }
            return ret;

        }



        public async Task<string> GetOrderInterfaceData(int orderDetailId)
        {

            try
            {
                 return await _orderRepository.GetInterfaceOrderData(orderDetailId);

            }
            catch (Exception ex)
            {
                return ex.Message;

            }

        }

        public async Task<string> GetOrderFolder(int orderDetailId)
        {

            try
            {
                return await _orderRepository.GetOrderFolder(orderDetailId);

            }
            catch (Exception ex)
            {
                return ex.Message;

            }

        }


        public async Task<int> SaveOrderNote(int orderDetailId,
                                    string note,
                                    string noteType,
                                    int statusId,
                                    string userName,
                                    string userType,
                                    int modifiedEmployeeId)
        {

            int retval;
            try
            {

                return await _orderRepository.SaveOrderNote(orderDetailId,
                                                            note,
                                                            noteType,
                                                            statusId,
                                                            userName,
                                                            userType,
                                                            modifiedEmployeeId);

            }
            catch (System.Exception ex)
            {
                retval = 0;
            }


            return retval;


        }



        #endregion

    }
}


