using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program
{
    static async Task Main(string[] args)
    {
        var token = "7223289386:AAFUBQhzDRLm4EYVzdVbybwcSZ9klbcGOBA"; // замените на ваш токен

        var host = new HostBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton(new Bot(token));
                services.AddHostedService<BotService>();
            })
            .Build();

        await host.RunAsync();
    }
}

public class BotService : IHostedService
{
    private readonly Bot _bot;

    public BotService(Bot bot)
    {
        _bot = bot;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _bot.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}