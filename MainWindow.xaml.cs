using FileHelperService;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace CopyFile;

public partial class MainWindow : Window
{
    private FileHelper _fileHelper;
    private Progress<int> _progress = new Progress<int>();

    public MainWindow()
    {
        InitializeComponent();
        _progress.ProgressChanged +=
            (s, args) =>
            {
                Progress.Value += args;
            };
    }

    private async void CopyFiles_click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!String.IsNullOrEmpty(OldPathTB.Text)
                && !String.IsNullOrEmpty(newPathTB.Text))
            {
                _fileHelper = new(OldPathTB.Text);
                Progress.Maximum = _fileHelper.Files.Count();
                await _fileHelper.MoveToOtherDirectoryParallelAsync(newPathTB.Text, _progress);
            }
        }
        catch (IOException ex)
        {
            _errorCopy(ex.Message);
            return;
        }

        _succesCopy();
    }

    private void _errorCopy(string msg)
    {
        IndicatorLabel.Foreground = Brushes.DarkRed;
        IndicatorLabel.Content = msg;
    }

    private void _succesCopy()
    {
        IndicatorLabel.Foreground = Brushes.Green;
        IndicatorLabel.Content = "Succes";
    }
}