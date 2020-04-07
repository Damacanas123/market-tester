using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackOfficeEngine.MessageEnums;

namespace MarketTester.Model.Scheduler
{
    public class Matcher
    {
        private class PassiveOrder
        {
            public decimal qty;
            public decimal price;
            public string symbol;
            public string orderId;
            public Side side;
        }
        private class SymbolMarket
        {
            public Dictionary<string, decimal> orderIdMatchedQuantityMap = new Dictionary<string, decimal>();
            public List<PassiveOrder> buys = new List<PassiveOrder>();
            public List<PassiveOrder> sells = new List<PassiveOrder>();
            public string symbol;
            /// <summary>
            /// Add new message to market and return matching quantity
            /// </summary>
            /// <param name="orderId"></param>
            /// <param name="initialQty"></param>
            /// <param name="price"></param>
            /// <param name="side"></param>
            /// <returns>Execution quantity for this new order</returns>
            public decimal AddNewMessage(string orderId,decimal initialQty,decimal price,Side side)
            {
                decimal qty = initialQty;
                if(side == Side.Buy)
                {

                    while(qty > 0 && sells.Count > 0 && sells[0].price <= price)
                    {
                        PassiveOrder nextPassive = sells[0];
                        if (!orderIdMatchedQuantityMap.ContainsKey(nextPassive.orderId))
                        {
                            orderIdMatchedQuantityMap[nextPassive.orderId] = 0;
                        }
                        if (!orderIdMatchedQuantityMap.ContainsKey(orderId))
                        {
                            orderIdMatchedQuantityMap[orderId] = 0;
                        }
                        if (qty >= nextPassive.qty)
                        {
                            orderIdMatchedQuantityMap[nextPassive.orderId] += nextPassive.qty;
                            orderIdMatchedQuantityMap[orderId] += nextPassive.qty;
                            qty -= nextPassive.qty;
                            sells.RemoveAt(0);                            
                        }
                        else
                        {
                            orderIdMatchedQuantityMap[nextPassive.orderId] += qty;
                            orderIdMatchedQuantityMap[orderId] += qty;
                            nextPassive.qty -= qty;
                            qty = 0;                            
                        }
                    }
                }
                else
                {
                    while (qty > 0 && buys.Count > 0 && buys[0].price >= price)
                    {
                        PassiveOrder nextPassive = buys[0];
                        if (!orderIdMatchedQuantityMap.ContainsKey(nextPassive.orderId))
                        {
                            orderIdMatchedQuantityMap[nextPassive.orderId] = 0;
                        }
                        if (!orderIdMatchedQuantityMap.ContainsKey(orderId))
                        {
                            orderIdMatchedQuantityMap[orderId] = 0;
                        }
                        if (qty >= nextPassive.qty)
                        {
                            orderIdMatchedQuantityMap[nextPassive.orderId] += nextPassive.qty;
                            orderIdMatchedQuantityMap[orderId] += nextPassive.qty;
                            qty -= nextPassive.qty;
                            buys.RemoveAt(0);
                        }
                        else
                        {
                            orderIdMatchedQuantityMap[nextPassive.orderId] += qty;
                            orderIdMatchedQuantityMap[orderId] += qty;
                            nextPassive.qty -= qty;
                            qty = 0;
                        }
                    }
                }
                if(qty > 0)
                {
                    PassiveOrder newPassive = new PassiveOrder { orderId = orderId, side = side, symbol = symbol, qty = qty, price = price };
                    AddPassiveOrder(newPassive);
                }
                return initialQty - qty;
            }

            private void AddPassiveOrder(PassiveOrder newPassive)
            {
                List<PassiveOrder> dummySides;
                if(newPassive.side == Side.Buy)
                {
                    dummySides = buys;
                }
                else
                {
                    dummySides = sells;
                }

                int index = 0;
                while (index < dummySides.Count && newPassive.price <= dummySides[index].price)
                {
                    index++;
                }
                dummySides.Insert(index, newPassive);
            }
            /// <summary>
            /// takes replace message arguements. qty is the quantity that is to be seen on the market.
            /// </summary>
            /// <param name="orderId"></param>
            /// <param name="qty"></param>
            /// <param name="price"></param>
            /// <param name="side"></param>
            /// <returns></returns>
            public decimal AddReplaceOrder(string orderId, decimal qty, decimal price, Side side)
            {
                
                AddCancelMessage(orderId, side);
                return AddNewMessage(orderId, qty, price, side);
            }

            public void AddCancelMessage(string orderId,Side side)
            {
                if (side == Side.Buy)
                {
                    int index = buys.FindIndex(new Predicate<PassiveOrder>((o) => o.orderId == orderId));
                    if(index != -1)
                    {
                        buys.RemoveAt(index);
                    }
                }
                else
                {
                    int index = sells.FindIndex(new Predicate<PassiveOrder>((o) => o.orderId == orderId));
                    if (index != -1)
                    {
                        sells.RemoveAt(index);
                    }
                }
            }

            
        }

        private Dictionary<string, SymbolMarket> marketMap = new Dictionary<string, SymbolMarket>();
        public Matcher() { }
        
        public decimal AddMessage(SchedulerRawItem item)
        {
            string symbol = item.Symbol;
            if(marketMap.TryGetValue(symbol,out SymbolMarket market))
            {
                
            }
            else
            {
                market = new SymbolMarket();
                marketMap[symbol] = market;
            }

            switch (item.MsgType)
            {
                case MsgType.New:
                    return market.AddNewMessage(item.SchedulerOrderID, item.OrderQty, item.Price, item.Side);
                case MsgType.Replace:
                    return market.AddReplaceOrder(item.SchedulerOrderID, item.OrderQty, item.Price, item.Side);
                case MsgType.Cancel:
                    market.AddCancelMessage(item.SchedulerOrderID, item.Side);
                    return 0;
                default:
                    throw new ArgumentException($"Message type {item.MsgType} is not valid for matcher");
            }
        }

        public decimal GetMatchedQuantity(SchedulerRawItem item)
        {
            if(!marketMap[item.Symbol].orderIdMatchedQuantityMap.TryGetValue(item.SchedulerOrderID,out decimal qty))
            {
                qty = 0m;
            }
            return qty;
        }
    }

    
}
