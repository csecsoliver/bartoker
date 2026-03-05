using System.Collections.Generic;

namespace Bartoker;

public static class DownloadManager
{
    public static List<DownloadItem> Downloads { get; set; } = [];

    public static string SaveLocation { get; set; } = "";

    public static void AddDownload(string url, bool convert)
    {
        Downloads.Add(new DownloadItem(url, convert, SaveLocation));
    }
}

