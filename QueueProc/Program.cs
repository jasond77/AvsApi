using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using ServiceInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace QueueProc
{
    internal class Program
    {
        static ServiceProvider services;
        static IMessageService _messageService;
        static IOrderService _orderService;
        static ILogService _logService;

        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                                   .AddJsonFile("appsettings.json", true, true)
                                   .Build();

            var serviceCol = new ServiceCollection();
            IoC.IocHelper.ConfigureServices(serviceCol, config);
            services = serviceCol.BuildServiceProvider();

            _logService = services.GetService<ILogService>();

            Task<int> s;
            if (args.Length < 1)
            {
                s = _logService.SaveLogEntry(1, "No body in message received from Queue: ", "");
                return;
            }



            s = _logService.SaveLogEntry(1, "Message received from Queue: " + args[0], "");
            ProcessMessageAsync(args[0]);




        }

        static void ProcessMessageAsync(string xmlMessage)
        {
            _messageService = services.GetService<IMessageService>();
            _orderService = services.GetService<IOrderService>();

            //<Update><Business_Interface_ID>1</Business_Interface_ID><Order_Detail_ID>123456</Order_Detail_ID><Update_Type>INSPECTED</Update_Type><Message></Message></Update>
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode xmlNode;
            int orderDetailId = 0;
            int businessInterfaceId = 0;
            int customerAccountId = 0;
            string updateType = "";
            string message = "";
            JToken avsData;
            string json = "";
            string url = "";
            string username = "";
            string password = "";
            string interfaceUniqueId = "";
            string uniqueId = "";
            int res;
            string orderFolder = "";
            try
            {

                xmlDoc.LoadXml(xmlMessage);
                xmlNode = xmlDoc.SelectSingleNode("Update/Business_Interface_ID");
                int.TryParse(xmlNode.InnerText, out businessInterfaceId);
                xmlNode = xmlDoc.SelectSingleNode("Update/Order_Detail_ID");
                int.TryParse(xmlNode.InnerText, out orderDetailId);
                xmlNode = xmlDoc.SelectSingleNode("Update/Update_Type");
                updateType = xmlNode != null ? xmlNode.InnerText : "";
                xmlNode = xmlDoc.SelectSingleNode("Update/Message");
                message = xmlNode != null ? xmlNode.InnerText : "";
                //load order data
                json = _orderService.GetOrderInterfaceData(orderDetailId).GetAwaiter().GetResult();
                if (json != "")
                {
                    avsData = JToken.Parse(json);
                    url = avsData["post_url"].ToString();
                    username = avsData["txt_interface_username"].ToString();
                    password = avsData["txt_interface_password"].ToString();
                    interfaceUniqueId = avsData["txt_interface_unique_id"].ToString();
                    uniqueId = avsData["unique_id"].ToString();
                    int.TryParse(avsData["customer_account_id"].ToString(), out customerAccountId);
                    Task<int> s = _logService.SaveLogEntry(1, "Message type: " + updateType
                                                         + "\nOrderDetailId: " + orderDetailId.ToString()
                                                         + "\nUniqueId: " + uniqueId
                                                         + "\nInterfaceUniqueId: " + interfaceUniqueId
                                                         + "\nurl: " + url
                                                         + "\nmessage: " + message
                                                         , "");

                    switch (updateType.ToUpper())
                    {
                        case "COMMENT":
                            _messageService.Send120Event(url,
                                                        1,
                                                        interfaceUniqueId,
                                                        uniqueId,
                                                        "",
                                                        "",
                                                        customerAccountId,
                                                        DateTime.Now,
                                                        message);

                            break;

                        case "ASSIGNED":
                            _messageService.Send121Event(url,
                                                    1,
                                                    interfaceUniqueId,
                                                    uniqueId,
                                                    "",
                                                    "",
                                                    customerAccountId,
                                                    DateTime.Now,
                                                    message);
                            break;
                        case "SCHEDULED":
                            break;
                        case "INSPECTED":
                            break;

                        case "HOLD":
                            _messageService.Send150Event(url,
                                                    1,
                                                    interfaceUniqueId,
                                                    uniqueId,
                                                    "",
                                                    "",
                                                    customerAccountId,
                                                    DateTime.Now,
                                                    message);
                            break;
                        case "CANCEL":
                            _messageService.Send170Event(url,
                                                    1,
                                                    interfaceUniqueId,
                                                    uniqueId,
                                                    "",
                                                    "",
                                                    customerAccountId,
                                                    DateTime.Now,
                                                    message);
                            break;
                        case "REACTIVATE":
                            _messageService.Send160Event(url,
                                                    1,
                                                    interfaceUniqueId,
                                                    uniqueId,
                                                    "",
                                                    "",
                                                    customerAccountId,
                                                    DateTime.Now,
                                                    message);
                            break;
                        case "SEND_REPORT":
                            System.Threading.Thread.Sleep(6000);     // wait for the invoice to print
                            orderFolder = _orderService.GetOrderFolder(orderDetailId).GetAwaiter().GetResult();

                            if (File.Exists(orderFolder + avsData["txt_customer_order_number"].ToString() + ".pdf") == false)
                            {
                                res = _orderService.SaveOrderNote(orderDetailId,
                                                                      avsData["txt_customer_order_number"].ToString() + ".pdf - does not exist, file not sent.",
                                                                      "admin",
                                                                      13,
                                                                      "interface",
                                                                      "customer",
                                                                      8).GetAwaiter().GetResult();

                            }
                            if (File.Exists(orderFolder + "invoice.pdf") == false)
                            {
                                res = _orderService.SaveOrderNote(orderDetailId,
                                                                      "invoice.pdf - does not exist, file not sent.",
                                                                      "admin",
                                                                      13,
                                                                      "interface",
                                                                      "customer",
                                                                      8).GetAwaiter().GetResult();

                            }
                            if (File.Exists(orderFolder + "invoice.pdf") == true)
                            {
                                res = _orderService.SaveOrderNote(orderDetailId,
                                                                      "files have been uploaded.",
                                                                      "admin",
                                                                      13,
                                                                      "interface",
                                                                      "customer",
                                                                      8).GetAwaiter().GetResult();

                            }
                            List<DataContracts.File> files = new List<DataContracts.File>();
                            DataContracts.File f = new DataContracts.File();
                            f.document = System.Convert.ToBase64String(System.IO.File.ReadAllBytes(orderFolder + avsData["txt_customer_order_number"].ToString() + ".pdf"));
                            f.encodingType = "Base64";
                            f.extension = "pdf";
                            f.name = "Appraisal";
                            f.type = "Appraisal";
                            files.Add(f);

                            f = new DataContracts.File();
                            f.document = System.Convert.ToBase64String(System.IO.File.ReadAllBytes(orderFolder + "invoice.pdf"));
                            f.encodingType = "Base64";
                            f.extension = "pdf";
                            f.name = "Invoice";
                            f.type = "Invoice";
                            files.Add(f);

                            _messageService.Send140Event(url,
                                                    1,
                                                    interfaceUniqueId,
                                                    uniqueId,
                                                    "",
                                                    "",
                                                    customerAccountId,
                                                    DateTime.Now,
                                                    files);

                            break;

                    }
                }



            }
            catch (Exception ex)
            {
                Task<int> s = _logService.SaveLogEntry(1, "Message from Queue not sent: " + updateType + "\n" + ex.Message + "\n" + orderFolder, interfaceUniqueId);


            }

        }
    }
}
