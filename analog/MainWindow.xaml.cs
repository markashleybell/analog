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

namespace analog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LogEntryCollection _data;

        public MainWindow()
        {
            InitializeComponent();

            _data = new LogEntryCollection();

            dataGrid.DataContext = _data;

            _data.CreateDatabase();

            _data.LoadDataFromFiles();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            _data.Query(textBox.Text);
        }
    }
}
