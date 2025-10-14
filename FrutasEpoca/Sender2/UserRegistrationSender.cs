using FrutasEpoca.Models;
using FrutasEpoca.RabbitMQ;
using System.Text.Json;

namespace FrutasEpoca.Sender2
{
    public static class UserRegistrationSender
    {
    public static async Task SendUserRegistrationAsync()
        {
            var user = new UserRegistrationMessage
            {
                FullName = "Jo�o da Silva",
                Address = "Rua das Laranjeiras, 123",
                RG = "12.345.678-9",
                CPF = "123.456.789-00",
                RegistrationDateTime = DateTime.Now
            };

            var message = JsonSerializer.Serialize(user);
            await RabbitMqService.PublishAsync(
                exchange: "users_exchange",
                routingKey: "users.validation",
                message: message
            ).ConfigureAwait(false);
            Console.WriteLine("Mensagem de registro de usu�rio enviada para valida��o.");
        }
    }
}
