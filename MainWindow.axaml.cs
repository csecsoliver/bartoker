using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MsBox.Avalonia;

namespace Bartoker;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DgDownloads.ItemsSource = DownloadManager.Downloads;
    }

    private void BtnSettings_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private async void BtnAdd_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var url = new Uri(TbUrl.Text!);
            
            DownloadManager.AddDownload(TbUrl.Text!, CbMp3.IsChecked!.Value);
        }
        catch (Exception error)
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", error.Message ).ShowAsync();
        }
        
    }
}