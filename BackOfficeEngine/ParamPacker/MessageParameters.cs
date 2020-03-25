using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackOfficeEngine.MessageEnums;

namespace BackOfficeEngine.ParamPacker
{
    public class NewMessageParameters
    {
        public ProtocolType protocolType;
        public string account;
        public string symbol;
        public decimal orderQty;
        public decimal price;
        public Side side;
        public OrdType ordType;
        public TimeInForce timeInForce;

        public NewMessageParameters(
            ProtocolType protocolType,
            string account,
            string symbol,
            decimal orderQty,
            Side side,
            TimeInForce timeInForce,
            OrdType ordType,
            decimal price)
        {
            this.protocolType = protocolType;
            this.account = account;
            this.symbol = symbol;
            this.orderQty = orderQty;
            this.side = side;
            this.timeInForce = timeInForce;
            this.ordType = ordType;
            this.price = price;
        }

        public NewMessageParameters(
            ProtocolType protocolType,
            string account,
            string symbol,
            decimal orderQty,
            Side side,
            TimeInForce timeInForce,
            OrdType ordType)
        {
            this.protocolType = protocolType;
            this.account = account;
            this.symbol = symbol;
            this.orderQty = orderQty;
            this.side = side;
            this.timeInForce = timeInForce;
            this.ordType = ordType;
            this.price = decimal.MaxValue;
        }
    }
    public struct ReplaceMessageParameters
    {
        public string nonProtocolID;
        public decimal orderQty;
        public decimal price;
        public ReplaceMessageParameters(string nonProtocolID,decimal orderQty,decimal price)
        {
            this.nonProtocolID = nonProtocolID;
            this.orderQty = orderQty;
            this.price = price;
        }
        public ReplaceMessageParameters(string nonProtocolID, decimal orderQty)
        {
            this.nonProtocolID = nonProtocolID;
            this.orderQty = orderQty;
            this.price = decimal.MaxValue;
        }
        public ReplaceMessageParameters(decimal price, string nonProtocolID)
        {
            this.nonProtocolID = nonProtocolID;
            this.orderQty = decimal.MaxValue;
            this.price = price;
        }
    }
    public struct CancelMessageParameters
    {
        public string nonProtocolID;
        public CancelMessageParameters(string nonProtocolID)
        {
            this.nonProtocolID = nonProtocolID;
        }
    }
}
