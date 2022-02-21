using Microsoft.EntityFrameworkCore;

namespace Di;

internal class AccountDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }

    public string DbPath { get; }

    public AccountDbContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "account.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
    }
}
