using FrutasEpoca.Sender1;
using FrutasEpoca.Sender2;

namespace FrutasEpoca
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            Task? validatorTask = null;
            Task? receiver1Task = null;
            Task? receiver2Task = null;

            Console.WriteLine("Menu:");
            Console.WriteLine("v - Iniciar Validator (consome validation queues)");
            Console.WriteLine("r1 - Iniciar Receiver 1 (frutas)");
            Console.WriteLine("r2 - Iniciar Receiver 2 (usuários)");
            Console.WriteLine("1 - Enviar fruta de época");
            Console.WriteLine("2 - Enviar registro de usuário");
            Console.WriteLine("q - Sair");

            while (!cts.IsCancellationRequested)
            {
                Console.Write(": ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    continue;

                input = input.Trim().ToLowerInvariant();
                if (input == "q")
                {
                    cts.Cancel();
                    break;
                }

                try
                {
                    switch (input)
                    {
                        case "v":
                            if (validatorTask is null || validatorTask.IsCompleted)
                            {
                                validatorTask = Task.Run(() => FrutasEpoca.Validation.MessageValidator.StartAsync(), cts.Token);
                                Console.WriteLine("Validator iniciado.");
                            }
                            else Console.WriteLine("Validator já em execução.");
                            break;
                        case "r1":
                            if (receiver1Task is null || receiver1Task.IsCompleted)
                            {
                                receiver1Task = Task.Run(() => FrutasEpoca.Receiver1.SeasonalFruitReceiver.StartAsync(), cts.Token);
                                Console.WriteLine("Receiver 1 iniciado.");
                            }
                            else Console.WriteLine("Receiver 1 já em execução.");
                            break;
                        case "r2":
                            if (receiver2Task is null || receiver2Task.IsCompleted)
                            {
                                receiver2Task = Task.Run(() => FrutasEpoca.Receiver2.UserRegistrationReceiver.StartAsync(), cts.Token);
                                Console.WriteLine("Receiver 2 iniciado.");
                            }
                            else Console.WriteLine("Receiver 2 já em execução.");
                            break;
                        case "1":
                            await SeasonalFruitSender.SendSeasonalFruitAsync().ConfigureAwait(false);
                            break;
                        case "2":
                            await UserRegistrationSender.SendUserRegistrationAsync().ConfigureAwait(false);
                            break;
                        default:
                            Console.WriteLine("Opção inválida.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro: {ex.Message}");
                }
            }

            Console.WriteLine("Saindo... aguardando tarefas finalizarem...");
            try { await Task.WhenAll(new[] { validatorTask ?? Task.CompletedTask, receiver1Task ?? Task.CompletedTask, receiver2Task ?? Task.CompletedTask }); }
            catch { }
        }
    }
}
