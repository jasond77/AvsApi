using DataContracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface
{
    public interface IMessageService
    {

        void Send100Event(string url,
                               int interfaceId,
                               string interfaceUniqueId,
                               string orderUniqueId,
                               string userName,
                               string password);

        void Send110Event(string url,
                                int interfaceId,
                                string interfaceUniqueId,
                                string orderUniqueId,
                                string userName,
                                string password,
                                decimal customerFee);

        void Send115Event(string url,
                          int interfaceId,
                          string interfaceUniqueId,
                          string userName,
                          string password,
                          string comment,
                          string description);

        void Send120Event(string url,
                                         int interfaceId,
                                         string interfaceUniqueId,
                                         string orderUniqueId,
                                         string userName,
                                         string password,
                int accountID,
                DateTime eventDate,
                             string note);

        void Send121Event(string url,
                          int interfaceId,
                          string interfaceUniqueId,
                          string orderUniqueId,
                          string userName,
                          string password,
                          int accountID,
                          DateTime eventDate,
                          string note);

        void Send130Event(string url,
                         int interfaceId,
                         string interfaceUniqueId,
                         string orderUniqueId,
                         string userName,
                         string password,
                int accountID,
                DateTime eventDate,
             List<File> documents);

        void Send140Event(string url,
                int interfaceId,
                string interfaceUniqueId,
                string orderUniqueId,
                string userName,
                string password,
                int accountID,
                DateTime eventDate,
               List<File> documents);

        void Send150Event(string url,
                 int interfaceId,
                 string interfaceUniqueId,
                 string orderUniqueId,
                 string userName,
                 string password,
                int accountID,
                DateTime eventDate,
                string note);


        void Send160Event(string url,
               int interfaceId,
               string interfaceUniqueId,
               string orderUniqueId,
               string userName,
               string password,
               int accountID,
               DateTime eventDate,
               string note);

        void Send170Event(string url,
                        int interfaceId,
                        string interfaceUniqueId,
                        string orderUniqueId,
                        string userName,
                        string password,
                int accountID,
                DateTime eventDate,
                   string note);

        void Send180Event(string url,
                       int interfaceId,
                       string interfaceUniqueId,
                       string orderUniqueId,
                       string userName,
                       string password,
                int accountID,
                DateTime eventDate,
                      string note);

        void SendAsyncResponse(string url,
                                      int interfaceId,
                                      string interfaceUniqueId,
                                      string orderUniqueId,
                                      string userName,
                                      string password,
                                      int accountID,
                                      DateTime eventDate,
                                      string comment,
                                      int eventCode,
                                      int statusCode);
    }
}
