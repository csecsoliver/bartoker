using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using System.Web;
using Microsoft.VisualBasic;

namespace Bartoker;

public class DownloadItem
{
    public int Id { get; set; }
    public AudioMeta Meta { get; set; }
    public string Status { get; set; }
    public double Progress { get; set; }
    public double Speed { get; set; }
    public string StatusText
    {
        get
        {
            return "";
        }
    }

    public string ProgressText
    {
        get
        {
            return "";
        }
    }

    public string SpeedText
    {
        get
        {
            return "";
        }
    }

    public bool ToConvert { get; set; }
    public bool Finished { get; set; } = false;

    public DownloadItem()
    {
        
    }
    public DownloadItem(string url, bool convert, string path)
    {
        var parsedUrl = new Uri(url);
        this.Meta = new AudioMeta();
        var provider = System.Globalization.CultureInfo.InvariantCulture;
        if (url.Contains("mr3.mp3"))
        {
            var urlPath = parsedUrl.AbsolutePath.Split("/");
            
            var format = "yyyyMMddHHmmss";
            if (urlPath.Length != 4 ||
                !DateTime.TryParseExact(urlPath[1], format, provider, default, out var startDate) ||
                !DateTime.TryParseExact(urlPath[2], format, provider, default, out var endDate))
            {
                throw new ArgumentException($"tried parsing as mp3 url, failed at path segments: {url}");
            }
            

            Meta.Start = startDate;
            Meta.End = endDate;
            Meta.Format = "mp3";
            Meta.Path = path;
            ToConvert = false;
        }
        else if (url.Contains("mr3.mp4"))
        {
            var urlQuery = parsedUrl.Query;
            var urlPath = parsedUrl.AbsolutePath.Split("/");
            var startTime = HttpUtility.ParseQueryString(urlQuery).Get("start");
            var endTime = HttpUtility.ParseQueryString(urlQuery).Get("end");
            var format = "yyyyMMdd";
            if (urlPath.Length != 6 || !DateTime.TryParseExact($"{urlPath[2]}{urlPath[3]}{urlPath[4]}", format, provider, default, out var date))
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
            Meta.Path = path;
            ToConvert = convert;
        }
        else
        {
            throw new ArgumentException($"not an mp3, not an mp4 url: {url}");
        }
    }
}