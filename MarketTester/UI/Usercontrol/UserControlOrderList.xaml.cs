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

namespace MarketTester.UI.Usercontrol
{
    /// <summary>
    /// Interaction logic for UserControlOrderList.xaml
    /// </summary>
    public partial class UserControlOrderList : UserControl
    {
        public UserControlOrderList()
        {
            InitializeComponent();
            DataGridOrders.Loaded += SetMinWidths; // I named my datagrid Griddy
        }

       

        private void DataGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is ScrollViewer)
            {
                ((DataGrid)sender).UnselectAll();
            }            
        }


        public void SetMinWidths(object source, EventArgs e)
        {
            foreach (var column in DataGridOrders.Columns)
            {
                column.MinWidth = column.ActualWidth;
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            }
        }
    }
}
