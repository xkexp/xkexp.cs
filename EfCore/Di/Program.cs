

using Di;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        // 方式一
        services.AddDbContext<AccountDbContext>();

        // 方式二
        services.AddDbContext<AccountDbContext2>(options =>
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            var dbpath = Path.Join(path, "account.db");
            options.UseSqlite($"Data Source={dbpath}");
        });
    })
    .Build();

using (var context = host.Services.GetService<AccountDbContext>())
{
    var accounts = context.Accounts.ToList();

    context.Accounts.Add(new Account()
    {
        Username = "hello111111",
        Password = "123456",
        PasswordSalt = "salt",
        CreateTime = DateTime.Now,
        Remark = "测试11111111111",
    });
    await context.SaveChangesAsync();
}


using (var context = host.Services.GetService<AccountDbContext2>())
{
    var accounts = context.Accounts.ToList();

    context.Accounts.Add(new Account()
    {
        Username = "hello22222222",
        Password = "123456",
        PasswordSalt = "salt",
        CreateTime = DateTime.Now,
        Remark = "测试22222222222222",
    });
    await context.SaveChangesAsync();
}

await host.RunAsync();