using EventContracts;
using MassTransit;

var busControl = Bus.Factory.CreateUsingRabbitMq(factory =>
{
    factory.Host("amqp://admin:password@192.168.1.2:5672", conf =>
    {
        conf.Username("admin");
        conf.Password("password");
    });
});
var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));
await busControl.StartAsync(source.Token);
try
{
    while (true)
    {
        string value = await Task.Run(() =>
        {
            Console.WriteLine("Enter message (or quit to exit)");
            Console.Write("> ");
            return Console.ReadLine();
        });

        if ("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
            break;

        await busControl.Publish<ValueEntered>(new
        {
            Value = value
        });
    }
}
finally
{
    await busControl.StopAsync();
}

namespace EventContracts
{
    public interface ValueEntered
    {
        string Value { get; }
    }
}

