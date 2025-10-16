using FrutasEpoca.Models;
using FrutasEpoca.RabbitMQ;
using System.Text.Json;

namespace FrutasEpoca.Sender1
{
    public static class SeasonalFruitSender
    {
    public static async Task SendSeasonalFruitAsync()
        {
            var fruits = new SeasonalFruitMessage[]
            {
                new SeasonalFruitMessage { FruitName = "Manga", Description = "Fruta tropical, doce e suculenta, típica do verão.", RequestDateTime = DateTime.Now },
                new SeasonalFruitMessage { FruitName = "Morango", Description = "Fruta vermelha, pequena e muito doce, muito usada em sobremesas.", RequestDateTime = DateTime.Now },
                new SeasonalFruitMessage { FruitName = "Abacaxi", Description = "Fruta cítrica, suculenta, com polpa amarela e sabor equilibrado entre doce e ácido.", RequestDateTime = DateTime.Now },
                new SeasonalFruitMessage { FruitName = "Maçã", Description = "Fruta crocante e suculenta, disponível em várias variedades.", RequestDateTime = DateTime.Now },
                new SeasonalFruitMessage { FruitName = "Mamão", Description = "Fruta macia e doce, rica em enzimas digestivas.", RequestDateTime = DateTime.Now }
            };

            foreach (var fruit in fruits)
            {
                var message = JsonSerializer.Serialize(fruit);
                await RabbitMqService.PublishAsync(
                    exchange: "fruits_exchange",
                    routingKey: "fruits.validation",
                    message: message
                ).ConfigureAwait(false);
                Console.WriteLine($"Mensagem enviada: {fruit.FruitName} - {fruit.RequestDateTime}");
                await Task.Delay(500).ConfigureAwait(false); // pequena pausa para observação
            }
        }
    }
}
