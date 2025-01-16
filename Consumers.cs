using MassTransit;

namespace ServerSideEvents.API;

public class InformationConsumer(ItemService ItemService) : IConsumer<InformationEvent>
{
    public Task Consume(ConsumeContext<InformationEvent> context)
    {
        ItemService.AddItemAvailable(
            new Information(
                new($"New item {Guid.NewGuid()}", Random.Shared.Next(0, 500))));
        return Task.CompletedTask;
    }
}

public class WarningConsumer(ItemService ItemService) : IConsumer<WarningEvent>
{
    public Task Consume(ConsumeContext<WarningEvent> context)
    {
        ItemService.AddItemAvailable(
            new Warning(
                new($"New item {Guid.NewGuid()}", Random.Shared.Next(0, 500))));
        return Task.CompletedTask;
    }
}