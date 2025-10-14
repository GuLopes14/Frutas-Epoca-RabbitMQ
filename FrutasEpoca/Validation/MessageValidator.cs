using FrutasEpoca.Models;
using FrutasEpoca.RabbitMQ;
using System.Text.Json;

namespace FrutasEpoca.Validation
{
    public static class MessageValidator
    {
        public static async Task StartAsync()
        {
            // Consumidor para frutas
            await RabbitMqService.ConsumeAsync(
                queue: "fruits_validation_queue",
                onMessage: async (body) =>
                {
                    var fruit = JsonSerializer.Deserialize<SeasonalFruitMessage>(body);
                    if (!string.IsNullOrWhiteSpace(fruit?.FruitName) && !string.IsNullOrWhiteSpace(fruit?.Description))
                    {
                        await RabbitMqService.PublishAsync(
                            exchange: "fruits_exchange",
                            routingKey: "fruits.receiver1",
                            message: body
                        ).ConfigureAwait(false);
                        Console.WriteLine("Fruta validada e encaminhada ao Receiver 1.");
                    }
                    else
                    {
                        Console.WriteLine("Mensagem de fruta inv�lida.");
                    }
                },
                exchange: "fruits_exchange",
                routingKey: "fruits.validation"
            ).ConfigureAwait(false);

            // Consumidor para usu�rios
            await RabbitMqService.ConsumeAsync(
                queue: "users_validation_queue",
                onMessage: async (body) =>
                {
                    var user = JsonSerializer.Deserialize<UserRegistrationMessage>(body);
                    if (!string.IsNullOrWhiteSpace(user?.FullName) &&
                        !string.IsNullOrWhiteSpace(user?.Address) &&
                        !string.IsNullOrWhiteSpace(user?.RG) &&
                        !string.IsNullOrWhiteSpace(user?.CPF))
                    {
                        await RabbitMqService.PublishAsync(
                            exchange: "users_exchange",
                            routingKey: "users.receiver2",
                            message: body
                        ).ConfigureAwait(false);
                        Console.WriteLine("Usu�rio validado e encaminhado ao Receiver 2.");
                    }
                    else
                    {
                        Console.WriteLine("Mensagem de usu�rio inv�lida.");
                    }
                },
                exchange: "users_exchange",
                routingKey: "users.validation"
            ).ConfigureAwait(false);
        }
    }
}
