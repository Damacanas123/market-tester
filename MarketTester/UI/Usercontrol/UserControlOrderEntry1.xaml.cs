using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Effects;
using MarketTester.Helper;

using BackOfficeEngine.MessageEnums;

namespace MarketTester.UI.Usercontrol
{
    public partial class UserControlOrderEntry1 : UserControl
    {
       

        private bool active = false;
        //private Side side = Side.BUY;

        //private Symbol currentSymbol;
        //private List<Symbol> symbols;
        public UserControlOrderEntry1()
        {
            InitializeComponent();
            //PopulateSymbolsComboBox();
            List<TimeInForce> itemsTimeInForce = CastIEnumerableToList(Enum.GetValues(typeof(TimeInForce)).Cast<TimeInForce>());
            itemsTimeInForce.RemoveAt(0);
            List<OrdType> itemsOrdType = CastIEnumerableToList(Enum.GetValues(typeof(OrdType)).Cast<OrdType>());
            itemsOrdType.RemoveAt(0);
            ComboBoxTimeInForce.ItemsSource = itemsTimeInForce;
            ComboBoxOrdType.ItemsSource = itemsOrdType;
        }

        private List<T> CastIEnumerableToList<T>(IEnumerable<T> e)
        {
            return new List<T>(e);
        }

        
        
        
        //private void PopulateSymbolsComboBox()
        //{
        //    symbols = Symbols.LoadSymbols();
        //    foreach (Symbol symbol in symbols)
        //    {
        //        comboSymbol.Items.Add(symbol);
        //    }
        //    comboSymbol.SelectedIndex = 0;
        //    currentSymbol = symbols[0];
        //}

               
    }


}
