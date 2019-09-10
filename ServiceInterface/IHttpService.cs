using DataContracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceInterface
{
    public interface IHttpService
    {
        HttpResponse PostMessage(string url,
                                string payload,
                                string userName,
                                string password);


    }
}
