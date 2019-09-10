using DataContracts;
using ServiceInterface;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class HttpService : IHttpService
    {

        #region[Dependencies]

        //private readonly IConfiguration _config;
        private ILogService _logService;
        public HttpService(ILogService logService)
        {
            _logService = logService;

        }


        #endregion

        #region[Public Methods]

        public HttpResponse PostMessage(string url, 
                                        string payload,
                                        string userName,
                                        string password)
        {
            HttpResponse response = new HttpResponse();
            Task<int> res;
            try
            {

            WebClient aWebClient = new WebClient();
            StringBuilder output = new StringBuilder();
                int i;

            System.Uri u = new System.Uri(url);
            if(userName != "" && password != "")
                {
                    NetworkCredential cr = new NetworkCredential(userName, password);
                    CredentialCache cc = new CredentialCache();
                    cc.Add(u, "Basic", cr);
                    aWebClient.Credentials = cc;
                }

                string log = "Post Message - " + url + "\n" + payload;
                res = _logService.SaveLogEntry(0, log, "");


                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
                aWebClient.Headers.Add("Content-Type", "application/json");
                aWebClient.Headers.Add("Accept", "application/json");
                byte[] byteArray = Encoding.Default.GetBytes(payload);
                byte[] resArray = aWebClient.UploadData(u, "POST", byteArray);
                response.headers = aWebClient.ResponseHeaders;
                response.responseData = Encoding.ASCII.GetString(resArray);
                response.success = true;
                log = "Post Response - " + url + "\n" + response.responseData;
                res = _logService.SaveLogEntry(0, log, "");

            }
            catch (Exception ex)
            {
                res = _logService.SaveLogEntry(0, response.responseData, "");
                response.responseData = ex.Message;
            }
            return response;

        }





        #endregion
    }
}
