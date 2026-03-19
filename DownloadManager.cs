using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Avalonia.Input;
using Downloader;
using Microsoft.EntityFrameworkCore;

namespace Bartoker;

public class DownloadManager
{
    public static DownloadManager Instance { get; } = new DownloadManager();
    private BartokerContext Context => BartokerContext.Instance;

    private string SaveLocation => Context.Settings.Find("SaveLocation")?.Value ?? Directory.GetCurrentDirectory();
    
    public DbSet<DownloadItem> Downloads => Context.Downloads;
    public DbSet<AudioMeta> AudioFiles => Context.AudioFiles;
    public ObservableCollection<DownloadItem> ObservableDownloads { get; set; }
    public ObservableCollection<AudioMeta> ObservableAudioFiles { get; set; }

    private DownloadManager()
    {
        Context.Database.EnsureCreated();
        Context.Downloads.Load();
        Context.AudioFiles.Load();
        ObservableDownloads = Context.Downloads.Local.ToObservableCollection();
        ObservableAudioFiles = Context.AudioFiles.Local.ToObservableCollection();
    }
    public void AddDownload(string url, bool convert)
    {
        
        Context.Downloads.Add(new DownloadItem(url, convert, SaveLocation));
        
        
    }
}

