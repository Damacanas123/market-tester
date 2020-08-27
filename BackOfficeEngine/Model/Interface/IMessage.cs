using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackOfficeEngine.MessageEnums;

namespace BackOfficeEngine.Model
{
    public interface IMessage
    {
        DateTime TimeStamp { get; set; }
        ProtocolType protocolType { get; set; }

        void RemoveGenericField(int field);

        #region getters
        MsgType GetMsgType();
        string GetClOrdID();
        string GetOrigClOrdID();
        string GetAccount();
        TimeInForce GetTimeInForce();
        bool IsOffHours();
        decimal GetOrderQty();
        decimal GetPrice();
        string GetSymbol();
        OrdType GetOrdType();
        Side GetSide();
        DateTime GetExecutionTime();
        decimal GetLastPx();
        DateTime GetSendingTime();
        DateTime GetTransactTime();
        decimal GetLastQty();
        decimal GetAvgPx();
        OrdStatus GetOrdStatus();
        string GetGenericField(int field);
        #endregion

        #region setters
        void SetClOrdID(string value);
        void SetOrigClOrdID(string value);
        void SetAccount(string value);
        void SetTimeInForce(TimeInForce value);
        void SetOffHours(bool value);
        void SetOrderQty(decimal value);
        void SetPrice(decimal value);
        void SetSymbol(string value);
        void SetOrdType(OrdType value);
        void SetSide(Side value);
        void SetLastPx(decimal value);
        void SetSendingTime(DateTime timestamp);
        void SetTransactTime(DateTime timestamp);
        void SetLastQty(decimal value);
        void SetAvgPx(decimal value);
        void SetOrdStatus(OrdStatus value);
        void SetGenericField(int field,string value);
        #endregion

        #region field checkers
        bool IsSetClOrdID();
        bool IsSetOrigClOrdID();
        bool IsSetAccount();
        bool IsSetTimeInForce();
        bool IsSetOrderQty();
        bool IsSetPrice();
        bool IsSetSymbol();
        bool IsSetOrdType();
        bool IsSetSide();
        bool IsSetExecutionTime();
        bool IsSetLastPx();
        bool IsSetSendingTime();
        bool IsSetTransactTime();
        bool IsSetLastQty();
        bool IsSetAvgPx();
        bool IsSetOrdStatus();
        bool IsSetGenericField(int field);
        #endregion

    }
}
