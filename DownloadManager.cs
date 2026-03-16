using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Input;
using Microsoft.EntityFrameworkCore;

namespace Bartoker;

public class DownloadManager
{
    public static DownloadManager Instance { get; } = new DownloadManager();
    private BartokerContext Context => BartokerContext.Instance;

    private string SaveLocation => Context.Settings.Find("SaveLocation")?.Value ?? "";
    
    public DbSet<DownloadItem> Downloads => Context.Downloads;
    public ObservableCollection<DownloadItem> Items { get; set; }

    private DownloadManager()
    {
        Context.Database.EnsureCreated();
        Context.Downloads.Load();
        Items = Context.Downloads.Local.ToObservableCollection();
    }
    public void AddDownload(string url, bool convert)
    {
        
        Context.Downloads.Add(new DownloadItem(url, convert, SaveLocation));
        
    }
}

