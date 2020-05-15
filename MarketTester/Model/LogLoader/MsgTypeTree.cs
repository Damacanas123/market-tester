using FixLogAnalyzer;
using MarketTester.Exceptions;
using Microsoft.Office.Interop.Excel;
using QuickFix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketTester.Model.LogLoader
{
    public class MsgTypeTree
    {
        public string MsgType { get; set; }
        public int OccurrenceNum { get; set; }
        /// <summary>
        /// dont ever edit this property or its sub properties outside of this class
        /// </summary>
        public Dictionary<int, TagTree> TagTrees { get; private set; } = new Dictionary<int, TagTree>();

        public MsgTypeTree()
        {

        }

        public void AddMessage(ExtendedLogMessage msg)
        {
            if(msg.GetField(Tags.MsgType) != MsgType)
            {
                throw new InvalidMessageType();
            }
            OccurrenceNum++;
            foreach(KeyValuePair<int,string> pair in msg.TagValuePairs)
            {
                if(!TagTrees.TryGetValue(pair.Key,out TagTree tagTree))
                {
                    tagTree = new TagTree();
                    TagTrees[pair.Key] = tagTree;
                }
                tagTree.Add(pair.Key, pair.Value, msg.LineNum);
            }
        }



        public class TagTree
        {
            public int Tag { get; set; }
            public int OccurrenceNum { get; set; }
            public Dictionary<string, ValueTree> ValueTrees { get; set; } = new Dictionary<string, ValueTree>();

            public void Add(int tag, string value,int lineNum)
            {
                if(tag != Tag)
                {
                    throw new UnmatchingTagException();
                }
                OccurrenceNum++;
                if(!ValueTrees.TryGetValue(value,out ValueTree valueTree)) 
                {
                    valueTree = new ValueTree();
                    ValueTrees[value] = valueTree;
                }
                valueTree.Add(value, lineNum);
            }
        }

        public class ValueTree
        {
            public string Value { get; set; }
            public int OccurrenceNum { get; set; }
            public List<int> LineNumbers { get; set; } = new List<int>();

            public void Add(string value,int lineNum)
            {
                if(value != Value)
                {
                    throw new UnmatchingValueException();
                }
                OccurrenceNum++;
                LineNumbers.Add(lineNum);
            }
        }
    }

}
