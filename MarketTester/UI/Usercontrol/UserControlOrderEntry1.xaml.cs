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

namespace MarketTester.UI.Usercontrol
{
    public partial class UserControlOrderEntry1 : UserControl
    {
        private static Brush background;
        private static Brush activeYellow;
       

        private bool active = false;
        //private Side side = Side.BUY;

        //private Symbol currentSymbol;
        //private List<Symbol> symbols;
        List<Control> controls;
        public UserControlOrderEntry1()
        {
            background = (Brush)FindResource("BackgroundPrimary");
            activeYellow = (Brush)FindResource("ActiveYellow");
            InitializeComponent();
            PopulateAccountComboBox();
            //PopulateSymbolsComboBox();
            controls = new List<Control>();
            controls.Add(comboAccount);
            

        }

        
        
        private void PopulateAccountComboBox()
        {
            comboAccount.Items.Add("Hesap 1");
            comboAccount.Items.Add("Hesap 2");
            comboAccount.SelectedIndex = 0;
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

        private void TextBoxPriceTextChanged(object sender, RoutedEventArgs e)
        {
            string text = textBoxPrice.Text;
            if (text != "")
            {
                text = Util.RemoveNonNumericKeepDot(text);
                textBoxPrice.Text = text;
                textBoxPrice.Select(text.Length, 0);
            }


        }
        private void TextBoxPriceOnClick(object s, RoutedEventArgs e)
        {
            textBoxPrice.Select(0, textBoxPrice.Text.Length);
        }

        private void ComboSymbolSelectionChanged(object sender, RoutedEventArgs e)
        {
            if(comboSymbol.SelectedIndex != -1)
            {
                //currentSymbol = symbols[comboSymbol.SelectedIndex];
                //textBoxPrice.Text = currentSymbol.GetLowerLimit().ToString();
                //labelPriceStep.Text = currentSymbol.GetPriceStep().ToString();
            }
        }

        
    }


}
