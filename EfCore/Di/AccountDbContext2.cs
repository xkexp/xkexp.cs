using Microsoft.EntityFrameworkCore;

namespace Di;

internal class AccountDbContext2 : DbContext
{
    public DbSet<Account> Accounts { get; set; }

    public string DbPath { get; }

    public AccountDbContext2()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "account.db");
    }

    public AccountDbContext2(DbContextOptions<AccountDbContext2> options) 
        : base(options)
    {
    }
}
