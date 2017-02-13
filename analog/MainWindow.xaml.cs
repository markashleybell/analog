using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private BackgroundWorker _worker;
        private OpenFileDialog _openFileDialog;

        public MainWindow()
        {
            InitializeComponent();

            _openFileDialog = new OpenFileDialog();
            _openFileDialog.Multiselect = true;
            _openFileDialog.Filter = "IIS Log Files|*.log";
            _openFileDialog.RestoreDirectory = true;

            _data = new LogEntryCollection();
            dataGrid.DataContext = _data;

            _data.CreateDatabaseIfNeeded();
            _data.ClearDatabase();
        }

        private void DoQuery(string text)
        {
            try
            {
                _data.Query(text);
                queryStatus.Text = string.Format(
                    "{0} row{1} matched",
                    _data.ResultCount,
                    _data.ResultCount == 1 ? "" : "s"
                );
            }
            catch(Exception ex)
            {
                queryStatus.Text = "Error";
                MessageBox.Show(ex.Message, "SQL Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            DoQuery(textBox.Text);
        }

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_openFileDialog.ShowDialog() == true)
            {
                _data.LoadDataFromFiles(_openFileDialog.FileNames);
                dataStatus.Text = string.Format(
                    "{0} file{1} loaded ({2} entries)", 
                    _openFileDialog.FileNames.Length,
                    _openFileDialog.FileNames.Length == 1 ? "" : "s",
                    _data.EntryCount
                );
                queryStatus.Text = "";
            }
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("SAVE");
        }

        private void SaveAsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("SAVEAS");
        }

        private void ExecuteQueryCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;   
        }

        private void ExecuteQueryCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DoQuery(textBox.Text);
        }

        private void ExitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
