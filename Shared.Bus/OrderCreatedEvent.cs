namespace Shared.Bus
{
    public record OrderCreatedEvent(Guid OrderId, Guid ProductId, int Count, Guid UserId);
}