using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Avalonia.Controls.Shapes;
using Avalonia.Threading;
using Downloader;
using Microsoft.VisualBasic;
using MsBox.Avalonia;
using Path = System.IO.Path;

namespace Bartoker;

public class DownloadItem : INotifyPropertyChanged
{
    public int Id { get; set; }
    public AudioMeta Meta { get; set; }
    public bool TodayOnly { get; init; }
    [NotMapped] public DownloadService? Downloader { get; set; }
    public bool Invalid { get; set; }
    [NotMapped] public string Error { get; set; } = "";
    public string SavedDownloader
    {
        get => JsonSerializer.Serialize(Downloader);
        init
        {
            var downloader = JsonSerializer.Deserialize<DownloadService>(value);
            if (downloader != null && (!TodayOnly || (TodayOnly && Meta.Start.Date == DateTime.Today)))
            {
                Downloader = downloader;
            }
            else
            {
                Error = "Nem folytatható a letöltés. Ajánlott az elem törlése.";
                Invalid = true;
            }
        }
    }

    public string StatusText => Downloader != null ? Downloader.Status.ToString() : string.Empty;

    [NotMapped] private Downloader.DownloadProgressChangedEventArgs? _progress;

    public string ProgressText => _progress != null
        ? $"{_progress.ProgressPercentage}% ({_progress.ReceivedBytesSize / 1000000}MB/{_progress.TotalBytesToReceive / 1000000}MB)"
        : string.Empty;

    public string SpeedText =>
        _progress != null ? $"{_progress.AverageBytesPerSecondSpeed / 1000000}MB/s" : string.Empty;


    public bool ToConvert { get; set; }
    public bool Finished { get; set; } = false;

    public DownloadItem()
    {

    }

    public DownloadItem(string url, bool convert, string basepath)
    {
        var parsedUrl = new Uri(url);
        this.Meta = new AudioMeta();
        var provider = System.Globalization.CultureInfo.InvariantCulture;
        if (url.Contains("mr3.mp3"))
        {
            var urlPath = parsedUrl.AbsolutePath.Split("/");

            const string format = "yyyyMMddHHmmss";
            if (urlPath.Length != 4 ||
                !DateTime.TryParseExact(urlPath[1], format, provider, default, out var startDate) ||
                !DateTime.TryParseExact(urlPath[2], format, provider, default, out var endDate))
            {
                throw new ArgumentException($"tried parsing as mp3 url, failed at path segments: {url}");
            }


            Meta.Start = startDate;
            Meta.End = endDate;
            Meta.Format = "mp3";
            Meta.Path = Path.Combine(basepath, Meta.DefaultFileName);
            Meta.SourceUrl = url;
            ToConvert = false;
            TodayOnly = true;
        }
        else if (url.Contains("mr3.mp4"))
        {
            var urlQuery = parsedUrl.Query;
            var urlPath = parsedUrl.AbsolutePath.Split("/");
            var startTime = HttpUtility.ParseQueryString(urlQuery).Get("start");
            var endTime = HttpUtility.ParseQueryString(urlQuery).Get("end");
            const string format = "yyyyMMdd";
            if (urlPath.Length != 6 || !DateTime.TryParseExact($"{urlPath[2]}{urlPath[3]}{urlPath[4]}", format,
                    provider, default, out var date))
            {
                throw new ArgumentException($"tried parsing as mp4 url, failed at path segments: {url}");
            }

            if (!double.TryParse(startTime, out var startTimeD) || !double.TryParse(endTime, out var endTimeD) ||
                TimeSpan.FromSeconds(startTimeD).Days >= 1 || TimeSpan.FromSeconds(endTimeD).Days >= 1)
            {
                throw new ArgumentException($"tried parsing as mp4 url, failed at query params: {url}");
            }

            Meta.Start = date.AddSeconds(startTimeD);
            Meta.End = date.AddSeconds(endTimeD);
            Meta.Format = "m4a";
            Meta.Path = Path.Combine(basepath, Meta.DefaultFileName);
            Meta.SourceUrl = url;
            ToConvert = convert;
            TodayOnly = false;
        }
        else
        {
            throw new ArgumentException($"not an mp3, not an mp4 url: {url}");
        }
        
        InitDownloader();
    }

    private async void InitDownloader()
    {
        try
        {
            var downloadOpt = new DownloadConfiguration()
            {
                ChunkCount = 8,
                ParallelDownload = true,
                ResumeDownloadIfCan = true,
            };
            Downloader = new DownloadService(downloadOpt);
            Downloader.DownloadProgressChanged += (s, e) =>
            {
                _progress = e;
                Dispatcher.UIThread.Post(() => NotifyPropertyChanged(nameof(ProgressText)));
                Dispatcher.UIThread.Post(() => NotifyPropertyChanged(nameof(SpeedText)));
                Dispatcher.UIThread.Post(() => NotifyPropertyChanged(nameof(StatusText)));
            };
            Downloader.DownloadFileCompleted += (s, e) =>
            {
                Finished = true;
                Dispatcher.UIThread.Post(() => NotifyPropertyChanged(nameof(Finished)));
                Dispatcher.UIThread.Post(() => NotifyPropertyChanged(nameof(StatusText)));
            };
            var file = Meta.Path;
            var url = Meta.SourceUrl;
            await Downloader.DownloadFileTaskAsync(url, file);
            
        }
        catch (Exception e)
        {
            Error = e.Message;
            Dispatcher.UIThread.Post(() => NotifyPropertyChanged(nameof(Error)));
            
        }
    }

public event PropertyChangedEventHandler? PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}