using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixLogAnalyzer
{
    public interface ILogAnalyzer
    {
        void Start();

        void SetInFilePath(string inFilePath);
        void SetOutFilePath(string outFilePath);
        void SetLogParser(IFixLineParser parser);

    }
}
