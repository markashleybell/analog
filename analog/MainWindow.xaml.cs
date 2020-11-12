using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace Analog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly LogEntryDataStore _db = new LogEntryDataStore();
        private readonly DataTable _data = new DataTable();
        private readonly BackgroundWorker _initWorker = new BackgroundWorker();
        private readonly BackgroundWorker _loadWorker = new BackgroundWorker();
        private readonly BackgroundWorker _queryWorker = new BackgroundWorker();
        private readonly OpenFileDialog _openFileDialog = new OpenFileDialog();

        public MainWindow()
        {
            InitializeComponent();

            _openFileDialog.Multiselect = true;
            _openFileDialog.Filter = "IIS Log Files|*.log";
            _openFileDialog.RestoreDirectory = true;

            _loadWorker.WorkerReportsProgress = true;
            _queryWorker.WorkerReportsProgress = true;

            _initWorker.DoWork += (sender, e) => {
                _db.CreateDatabaseIfNeeded();
                _db.ClearDatabase();
            };

            _initWorker.RunWorkerCompleted += (sender, e) => appStatus.Text = "Ready";

            _initWorker.RunWorkerAsync();

            _loadWorker.DoWork += (sender, e) => {
                _loadWorker.ReportProgress(0, "Loading...");
                var fileNames = (string[])e.Argument;
                try
                {
                    var loadCount = _db.PopulateDatabaseFromFiles(fileNames);
                    e.Result = new LoadResult {
                        Success = true,
                        Files = fileNames.Length,
                        Count = loadCount
                    };
                }
                catch (Exception ex)
                {
                    e.Result = new LoadResult {
                        Error = ex.Message
                    };
                }
            };

            _loadWorker.ProgressChanged += (sender, e) => loadStatus.Text = (string)e.UserState;

            _loadWorker.RunWorkerCompleted += (sender, e) => {
                var result = (LoadResult)e.Result;
                if (result.Success)
                {
                    loadStatus.Text = string.Format(
                        "{0} file{1} loaded ({2} entries)",
                        result.Files,
                        result.Files == 1 ? string.Empty : "s",
                        result.Count
                    );
                    queryStatus.Text = string.Empty;
                }
                else
                {
                    loadStatus.Text = "Error";
                    MessageBox.Show(result.Error, "SQL Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            _queryWorker.DoWork += (sender, e) => {
                _queryWorker.ReportProgress(0, "Running...");
                try
                {
                    var results = _db.Query((string)e.Argument);
                    e.Result = new QueryResult {
                        Success = true,
                        Results = results
                    };
                }
                catch (Exception ex)
                {
                    e.Result = new QueryResult {
                        Error = ex.Message
                    };
                }
            };

            _queryWorker.ProgressChanged += (sender, e) => queryStatus.Text = (string)e.UserState;

            _queryWorker.RunWorkerCompleted += (sender, e) => {
                var result = (QueryResult)e.Result;
                try
                {
                    var results = result.Results.DefaultView;
                    dataGrid.ItemsSource = results;
                    queryStatus.Text = string.Format(
                        "{0} row{1} matched",
                        results.Count,
                        results.Count == 1 ? string.Empty : "s"
                    );
                }
                catch (Exception ex)
                {
                    queryStatus.Text = "Error";
                    MessageBox.Show(ex.Message, "SQL Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
        }

        private void DoQuery(string text)
        {
            _data.Clear();
            _queryWorker.RunWorkerAsync(text);
        }

        private void Execute_Click(object sender, RoutedEventArgs e) => DoQuery(textBox.Text);

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_openFileDialog.ShowDialog() == true)
            {
                _loadWorker.RunWorkerAsync(_openFileDialog.FileNames);
            }
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e) => MessageBox.Show("SAVE");

        private void SaveAsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e) => MessageBox.Show("SAVEAS");

        private void ExecuteQueryCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void ExecuteQueryCommand_Executed(object sender, ExecutedRoutedEventArgs e) => DoQuery(textBox.Text);

        private void ExitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e) => Application.Current.Shutdown();

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            var selected = dataGrid.SelectedCells;

            var conditions = new List<string>();

            foreach (var cell in selected)
            {
                var header = cell.Column.Header;
                var content = cell.Column.GetCellContent(cell.Item) as TextBlock;

                conditions.Add(string.Format("{0} = '{1}'", header, content.Text));
            }

            var whereClause = " or ";

            if (!textBox.Text.Contains("where", StringComparison.InvariantCultureIgnoreCase))
            {
                whereClause = " where ";
            }

            whereClause += string.Join(" or ", conditions.ToArray());

            textBox.Text += whereClause;
        }
    }
}
