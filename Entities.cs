using MassTransit;

namespace ServerSideEvents.API;

public record Item(string Name, double Value);
public record Message(Item Item);
public record Information(Item Item) : Message(Item);
public record Warning(Item Item) : Message(Item);

public record WarningEvent(Guid CorrelationId, Item Item) : CorrelatedBy<Guid>;
public record InformationEvent(Guid CorrelationId, Item Item) : CorrelatedBy<Guid>;