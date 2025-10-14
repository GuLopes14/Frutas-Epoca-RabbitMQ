using FrutasEpoca.Models;
using FrutasEpoca.RabbitMQ;
using System.Text.Json;

namespace FrutasEpoca.Receiver1
{
    public static class SeasonalFruitReceiver
    {
        public static async Task StartAsync()
        {
            await RabbitMqService.ConsumeAsync(
                queue: "fruits_receiver1_queue",
                onMessage: (body) =>
                {
                    var fruit = JsonSerializer.Deserialize<SeasonalFruitMessage>(body);
                    Console.WriteLine($"[Receiver 1] Fruta recebida: {fruit.FruitName} - {fruit.Description} ({fruit.RequestDateTime})");
                }
            ,
                exchange: "fruits_exchange",
                routingKey: "fruits.receiver1"
            ).ConfigureAwait(false);
        }
    }
}
