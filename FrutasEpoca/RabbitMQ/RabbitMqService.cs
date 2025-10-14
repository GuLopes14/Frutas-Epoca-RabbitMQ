using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace FrutasEpoca.RabbitMQ
{
    public static class RabbitMqService
    {
        private static readonly ConnectionFactory factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        public static void Publish(string exchange, string routingKey, string message)
        {
            PublishAsync(exchange, routingKey, message).GetAwaiter().GetResult();
        }

        public static async Task PublishAsync(string exchange, string routingKey, string message, CancellationToken cancellationToken = default)
        {
            await using var connection = await factory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
            await using var channel = await connection.CreateChannelAsync((global::RabbitMQ.Client.CreateChannelOptions?)null, cancellationToken).ConfigureAwait(false);
            await channel.ExchangeDeclareAsync(exchange, "direct", true, false, null, false, cancellationToken).ConfigureAwait(false);

            var body = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync(exchange, routingKey, body.AsMemory(), cancellationToken).ConfigureAwait(false);
        }

        public static void Consume(string queue, Action<string> onMessage)
        {
            ConsumeAsync(queue, onMessage).GetAwaiter().GetResult();
        }

    public static async Task ConsumeAsync(string queue, Action<string> onMessage, string? exchange = null, string? routingKey = null, CancellationToken cancellationToken = default)
        {
            var connection = await factory.CreateConnectionAsync(cancellationToken).ConfigureAwait(false);
            var channel = await connection.CreateChannelAsync((global::RabbitMQ.Client.CreateChannelOptions?)null, cancellationToken).ConfigureAwait(false);

            await channel.QueueDeclareAsync(queue, true, false, false, null, false, cancellationToken).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(exchange) && !string.IsNullOrEmpty(routingKey))
            {
                // Ensure exchange exists (declare) and bind the queue
                await channel.ExchangeDeclareAsync(exchange, "direct", true, false, null, false, cancellationToken).ConfigureAwait(false);
                await channel.QueueBindAsync(queue, exchange, routingKey, null, false, cancellationToken).ConfigureAwait(false);
            }
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += (model, ea) =>
            {
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                onMessage(body);
                channel.BasicAckAsync(ea.DeliveryTag, false, cancellationToken).GetAwaiter().GetResult();
                return Task.CompletedTask;
            };
            await channel.BasicConsumeAsync(queue, false, consumer, cancellationToken).ConfigureAwait(false);
            Console.WriteLine($"Consumindo fila: {queue}");
        }
    }
}
