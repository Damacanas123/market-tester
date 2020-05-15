using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketTester.Model.LogLoader
{
    public class RequestResponses
    {
        //first message that contains a new clOrdId will be assumed as a request
        public ExtendedLogMessage Request { get; set; }
        //all messages with the same ClOrdID that comes after the request message will be assumed as responses
        public List<ExtendedLogMessage> Responses { get; set; }

        public void AddMessage(ExtendedLogMessage msg)
        {
            if( Request == null)
            {
                Request = msg;
            }
            else
            {
                Responses.Add(msg);
            }
        }


    }
}
