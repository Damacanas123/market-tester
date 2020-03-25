using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MarketTester.Enumeration;

namespace MarketTester.ViewModel.Manager
{
    public class InfoManager
    {        
        public static ConcurrentBag<ViewModelInfoBox> InfoViewModels = new ConcurrentBag<ViewModelInfoBox>();

        public static void PublishInfo(EInfo infoType,string message)
        {
            foreach(ViewModelInfoBox vm in InfoViewModels)
            {
                if(vm.InfoPriority == infoType)
                    vm.InfoText += "--> " + message + "\n";
            }
        }
    }
}
