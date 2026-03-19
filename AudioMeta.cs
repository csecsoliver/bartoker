using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.VisualBasic;

namespace Bartoker;

public class AudioMeta
{
    private const string DateFormat = @"\Yyyyy\MMM\Ddd\Hhh\Mmm\Sss";
    public int Id  { get; set; }
    public string SourceUrl { get; set; } // https://hangtar-cdn.connectmedia.hu/20260302000000/20260302060000/mr3.mp3 if today or
                                          // https://hangtar-cdn.connectmedia.hu/hangtar/2026/02/02/mr3.mp4?start=0&end=21600 is the one it redirects to if not today
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Format { get; set; }
    public string Path { get; set; }

    public string DefaultFileName => $"{Start.ToString(DateFormat)}-{End.ToString(DateFormat)}.{Format}";

    public long SizeBytes { get; set; } // bytes
    [NotMapped]
    public Dictionary<string, string> ID3
    {
        get
        {
            return new Dictionary<string, string>();
        }
        set
        {
            throw new NotImplementedException();
        }
    }

}