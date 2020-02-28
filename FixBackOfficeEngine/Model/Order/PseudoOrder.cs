using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackOfficeEngine.MessageEnums;
using BackOfficeEngine.Helper.IdGenerator;
using BackOfficeEngine.ParamPacker;

namespace BackOfficeEngine.Model
{
    internal class PseudoOrder : BaseOrder
    {
        protected PseudoOrder(IMessage newOrderMessage, string non_protocol_pseudo_id) : base(newOrderMessage, non_protocol_pseudo_id)
        {

        }

        protected PseudoOrder() { }

        protected PseudoOrder(BaseOrder other) : base(other)
        {

        }

        internal new static (IMessage,PseudoOrder) CreateNewOrder(NewMessageParameters prms,string nonProtocolPseudoID)
        {
            IMessage newOrderMessage;
            BaseOrder baseOrder;
            (newOrderMessage, baseOrder) = BaseOrder.CreateNewOrder(prms, nonProtocolPseudoID);
            PseudoOrder order = new PseudoOrder(baseOrder);
            return (newOrderMessage, order);
        }

        

        internal override IMessage PrepareReplaceMessage(ReplaceMessageParameters prms)
        {
            IMessage msg = base.PrepareReplaceMessage(prms);
            origClOrdID = msg.GetOrigClOrdID();
            clOrdID = msg.GetClOrdID();
            return msg;
        }

        internal override IMessage PrepareCancelMessage(CancelMessageParameters prms)
        {
            IMessage msg = base.PrepareCancelMessage(prms);
            origClOrdID = msg.GetOrigClOrdID();
            clOrdID = msg.GetClOrdID();
            return msg;
        }
    }
}
