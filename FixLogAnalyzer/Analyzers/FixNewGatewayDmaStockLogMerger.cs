using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;



namespace FixLogAnalyzer
{
    public class DmaStockMerged
    {
        public RequestAckPair dmaPair;
        public List<RequestAckPair> stockPairs = new List<RequestAckPair>();
        public TimeSpan stockTotalDiff
        {
            get
            {
                if(stockPairs.Count > 1)
                {
                    return (stockPairs[stockPairs.Count - 1].acknowledge.logtime - stockPairs[0].request.logtime);
                }
                else
                {
                    return stockPairs[0].acknowledge.logtime - stockPairs[0].request.logtime;
                }
            }
        }

        public long stockTotalDiffMicro
        {
            get
            {
                return stockTotalDiff.Ticks / 10;
            }
        }

        public DmaStockMerged(RequestAckPair dmaPair)
        {
            this.dmaPair = dmaPair;
        }
    }
    public class FixNewGatewayDmaStockLogMerger : ILogAnalyzer
    {
        private string dmaLogPath;
        private string stockLogPath;
        private IFixLineParser parser;
        private string outFilePath;
        private List<RequestAckPair> dmaPairs;
        private List<RequestAckPair> stockPairs;

        public FixNewGatewayDmaStockLogMerger() { }
        public FixNewGatewayDmaStockLogMerger(string dmaLogPath,string stockLogPath,IFixLineParser parser,string outFilePath)
        {
            this.dmaLogPath = dmaLogPath;
            this.stockLogPath = stockLogPath;
            this.parser = parser;
            this.outFilePath = outFilePath;
        }

        private Dictionary<string,DmaStockMerged> Merge()
        {
            TimeDiffAnalyzer diffAnalyzer = new TimeDiffAnalyzer(dmaLogPath, "dummy", parser);
            dmaPairs = diffAnalyzer.StartInMemory();
            diffAnalyzer.SetInFilePath(stockLogPath);
            stockPairs = diffAnalyzer.StartInMemory();
            Dictionary<string, DmaStockMerged> mapping = new Dictionary<string, DmaStockMerged>();
            foreach(RequestAckPair pair in dmaPairs)
            {
                mapping[pair.request.clOrdIDClippedFromRight] = new DmaStockMerged(pair);
            }
            foreach(RequestAckPair pair in stockPairs)
            {
                string allocID = Util.GetTag(pair.request.msg, "70");
                mapping[allocID].stockPairs.Add(pair);
            }
            return mapping;
        }

        public void Start()
        {
            Dictionary<string, DmaStockMerged> mapping = Merge();
            using (StreamWriter writer = new StreamWriter(outFilePath))
            {
                writer.WriteLine("MsgType,Symbol,OrderQty,Side,Dma Delay Micro,Stock Delay Diff Micro,Internal Delay,Dma Request Time,Dma Acknowledge Time,Stock Request Times," +
                    "Stock Ack Times,Dma Request,Dma Acknowledge,Stock Requests,Stock Acknowledges");
                //note that dmaPairs keep their time ordering.
                foreach (RequestAckPair dmaPair in dmaPairs)
                {
                    string msgType = Util.GetTag(dmaPair.request.msg, "35");
                    string symbol = Util.GetTag(dmaPair.request.msg, "55");
                    string orderQty = Util.GetTag(dmaPair.request.msg, "38");
                    string side = Util.GetSidePretty(dmaPair.request.msg);
                    long dmaDelay = dmaPair.diffMicro;
                    
                    if (!mapping.ContainsKey(dmaPair.request.clOrdIDClippedFromRight))
                    {
                        continue;
                    }
                    DmaStockMerged correspondingMerged = mapping[dmaPair.request.clOrdIDClippedFromRight];
                    //means that order is rejected by gateway
                    if(correspondingMerged.stockPairs.Count == 0)
                    {
                        continue;
                    }
                    long stockDelay = correspondingMerged.stockTotalDiffMicro;
                    string dmaRequestTime = dmaPair.request.logtime.ToString(Util.outDateFormat);
                    string dmaAckTime = dmaPair.acknowledge.logtime.ToString(Util.outDateFormat);
                    
                    string stockRequestTimes = "";
                    string stockAckTimes = "";
                    string stockRequests = "";
                    string stockAcks = "";
                    foreach (RequestAckPair stockPair in correspondingMerged.stockPairs)
                    {
                        stockRequestTimes += stockPair.request.logtime.ToString(Util.outDateFormat) + "|";
                        stockAckTimes += stockPair.acknowledge.logtime.ToString(Util.outDateFormat) + "|";
                        stockRequests += stockPair.request.msg;
                        stockAcks += stockPair.acknowledge.msg;
                    }
                    stockRequestTimes = stockRequestTimes.Substring(0, stockRequestTimes.Length - 1);
                    stockAckTimes = stockAckTimes.Substring(0, stockAckTimes.Length - 1);
                    stockRequests = stockRequests.Substring(0, stockRequests.Length - 1);
                    stockAcks = stockAcks.Substring(0, stockAcks.Length - 1);
                    string dmaRequest = dmaPair.request.msg;
                    string dmaAcknowledge = dmaPair.acknowledge.msg;
                    writer.WriteLine($"{msgType},{symbol},{orderQty},{side},{dmaDelay},{stockDelay},{dmaDelay - stockDelay},{dmaRequestTime},{dmaAckTime},{stockRequestTimes}," +
                        $"{stockAckTimes},{dmaRequest},{dmaAcknowledge},{stockRequests},{stockAcks}");

                }
            }
        }

        public override string ToString()
        {
            return nameof(FixNewGatewayDmaStockLogMerger);
        }

        public void SetDmaPath(string path)
        {
            this.dmaLogPath = path;
        }

        public void SetStockPath(string path)
        {
            this.stockLogPath = path;
        }

        public void SetOutFilePath(string path)
        {
            this.outFilePath = path;
        }

        public void SetInFilePath(string inFilePath)
        {
            this.dmaLogPath = inFilePath;
        }

        public void SetLogParser(IFixLineParser parser)
        {
            this.parser = parser;
        }
    }
}
