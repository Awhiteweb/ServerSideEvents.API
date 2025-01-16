using MassTransit;

namespace ServerSideEvents.API;

public class Worker(IBus _bus) : BackgroundService {
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var id = 0;
        while(!stoppingToken.IsCancellationRequested) {
            if(id % 2 == 0) {
                await _bus.Publish(
                    new InformationEvent(
                        Guid.NewGuid(),
                        new(
                            $"Message {id}", 
                            Random.Shared.NextDouble())), 
                    stoppingToken);
            }
            else {
                await _bus.Publish(
                    new WarningEvent(
                        Guid.NewGuid(),
                        new(
                            $"Message {id}", 
                            Random.Shared.NextDouble())), 
                    stoppingToken);
            }
            id++;
            await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(1,10)), stoppingToken);
        }
    }
}