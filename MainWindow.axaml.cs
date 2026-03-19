using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using MsBox.Avalonia;

namespace Bartoker;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        DgDownloads.ItemsSource = DownloadManager.Instance.ObservableDownloads;
        
        
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
            
            DownloadManager.Instance.AddDownload(TbUrl.Text!, CbMp3.IsChecked!.Value);
        }
        catch (Exception error)
        {
            await MessageBoxManager.GetMessageBoxStandard("Error", error.Message).ShowAsPopupAsync(this);
        }
        
    }

    private async void DgDownloads_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if(e.Source is not Border source || sender is not DataGrid datagrid) return;
        switch (source.Name)
        {
            case "CellBorder":
                await MessageBoxManager.GetMessageBoxStandard("asd", "asd" ).ShowAsPopupAsync(this);
                break;
            case "HeaderBackground":
                return;
        }
    }
}