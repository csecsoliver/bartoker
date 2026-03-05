using System;
using Microsoft.EntityFrameworkCore;

namespace Bartoker;

public class BartokerContext : DbContext
{
    public DbSet<DownloadItem> Downloads { get; set; }
    public DbSet<Setting> Settings { get; set; }
    private string DbPath { get; }

    public BartokerContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Combine(path, "Bartoker.sqlite3");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
}