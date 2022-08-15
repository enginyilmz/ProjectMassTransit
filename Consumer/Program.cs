using EventContracts;
using MassTransit;

var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("amqp://admin:password@192.168.1.2:5672", conf =>
    {
        conf.Username("admin");
        conf.Password("password");
    });

    cfg.ReceiveEndpoint("event-listener", e =>
    {
        e.Consumer<EventConsumer>();
    });
});

var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

await busControl.StartAsync(source.Token);
try
{
    Console.WriteLine("Press enter to exit");

    await Task.Run(() => Console.ReadLine());
}
finally
{
    await busControl.StopAsync();
}

class EventConsumer :
            IConsumer<ValueEntered>
{
    public async Task Consume(ConsumeContext<ValueEntered> context)
    {
        Console.WriteLine("Value: {0}", context.Message.Value);
    }
}

namespace EventContracts
{
    public interface ValueEntered
    {
        string Value { get; }
    }
}

