using FrutasEpoca.Models;
using FrutasEpoca.RabbitMQ;
using System.Text.Json;

namespace FrutasEpoca.Receiver2
{
    public static class UserRegistrationReceiver
    {
        public static async Task StartAsync()
        {
            await RabbitMqService.ConsumeAsync(
                queue: "users_receiver2_queue",
                onMessage: (body) =>
                {
                    var user = JsonSerializer.Deserialize<UserRegistrationMessage>(body);
                    Console.WriteLine($"[Receiver 2] Usuário recebido: {user.FullName}, {user.Address}, RG: {user.RG}, CPF: {user.CPF}, Registrado em: {user.RegistrationDateTime}");
                }
            ,
                exchange: "users_exchange",
                routingKey: "users.receiver2"
            ).ConfigureAwait(false);
        }
    }
}
