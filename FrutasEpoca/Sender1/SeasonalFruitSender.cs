using FrutasEpoca.Models;
using FrutasEpoca.RabbitMQ;
using System.Text.Json;

namespace FrutasEpoca.Sender1
{
    public static class SeasonalFruitSender
    {
    public static async Task SendSeasonalFruitAsync()
        {
            var fruit = new SeasonalFruitMessage
            {
                FruitName = "Manga",
                Description = "Fruta tropical, doce e suculenta, típica do verão.",
                RequestDateTime = DateTime.Now
            };

            var message = JsonSerializer.Serialize(fruit);
            await RabbitMqService.PublishAsync(
                exchange: "fruits_exchange",
                routingKey: "fruits.validation",
                message: message
            ).ConfigureAwait(false);
            Console.WriteLine("Mensagem de fruta de �poca enviada para validação.");
        }
    }
}
