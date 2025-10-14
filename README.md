
# FrutasEpoca RabbitMQ Demo

## Visão Geral

Este projeto demonstra o uso de RabbitMQ com .NET 8 para troca de mensagens entre dois Senders, um serviço de validação e dois Receivers. O cenário simula:

- Sender 1: envia mensagens sobre frutas de época (nome, descrição e data/hora da solicitação) para validação.
- Sender 2: envia registros de usuário (nome, endereço, RG, CPF e data/hora de registro) para validação.
- Validation: consome as mensagens de validação, verifica campos mínimos e encaminha para os Receivers apropriados.
- Receiver 1: recebe frutas validadas.
- Receiver 2: recebe registros de usuário validados.

O projeto já configura exchanges, filas e routing keys usadas no fluxo.

---

## Estrutura e mapeamento

- Exchange `fruits_exchange`
  - routing key `fruits.validation` → queue `fruits_validation_queue` (Validation)
  - routing key `fruits.receiver1` → queue `fruits_receiver1_queue` (Receiver 1)

- Exchange `users_exchange`
  - routing key `users.validation` → queue `users_validation_queue` (Validation)
  - routing key `users.receiver2` → queue `users_receiver2_queue` (Receiver 2)

Validação (implementada em `Validation/MessageValidator.cs`):

- Frutas: verifica se `FruitName` e `Description` não estão vazios e encaminha para `fruits.receiver1`.
- Usuários: verifica se `FullName`, `Address`, `RG` e `CPF` não estão vazios e encaminha para `users.receiver2`.

---

## Pré-requisitos

- .NET SDK 8 (ou compatível com o TargetFramework do projeto)
- Docker Desktop (para subir o RabbitMQ localmente)
- Git (opcional, para versionamento)

Recomendo abrir o PowerShell como administrador para operações com Docker/Git quando necessário.

---

## Rodando com Docker Compose

O repositório inclui `docker-compose.yml` que inicia um container RabbitMQ com a interface de gerenciamento.

1) Inicie o Docker Desktop (garanta que o backend Linux/WSL2 esteja funcionando).

2) No diretório raiz do projeto, execute:

```powershell
docker compose up -d
```

Isto criará o container e mapeará as portas:

- 5672 → AMQP (broker)
- 15672 → Management UI (HTTP)

3) Abra o painel de gerenciamento em http://localhost:15672 (usuário/senha: guest/guest)

---

## Compilar e executar a aplicação (.NET)

1) Restaurar, compilar e executar a aplicação:

```powershell
dotnet build .\FrutasEpoca\FrutasEpoca.csproj
dotnet run --project .\FrutasEpoca\FrutasEpoca.csproj
```

2) A aplicação abre um menu interativo. Opções principais:

- `v` → iniciar o Validator (consome filas de validação)
- `r1` → iniciar Receiver 1 (frutas)
- `r2` → iniciar Receiver 2 (usuários)
- `1` → enviar uma mensagem de fruta de época (executa Sender 1)
- `2` → enviar um registro de usuário (executa Sender 2)
- `q` → sair

Você pode abrir múltiplas instâncias do binário (ou executar em terminais distintos) para rodar receivers e enviar mensagens simultaneamente.

---

## Testes e verificação no RabbitMQ Management

Depois de subir o container e iniciar os receivers/validator via menu:

- Abra http://localhost:15672 e faça login com `guest` / `guest`.
- Em "Queues" verifique se as filas foram criadas (`fruits_validation_queue`, `fruits_receiver1_queue`, `users_validation_queue`, `users_receiver2_queue`).
- Em "Exchanges" verifique `fruits_exchange` e `users_exchange`.
- Em "Connections" e "Channels" você verá as conexões abertas pela aplicação enquanto os componentes estiverem em execução.

Para enviar testes, use as opções `1` e `2` no menu da aplicação e observe as mensagens fluindo: primeiro para as filas de validação, depois para as filas dos receivers (após validação).

---

## Detalhes de implementação (pontos importantes)

- O cliente RabbitMQ usado é `RabbitMQ.Client` (v7). As APIs são assíncronas (CreateConnectionAsync, CreateChannelAsync, ExchangeDeclareAsync, BasicPublishAsync, QueueDeclareAsync, BasicConsumeAsync, BasicAckAsync). O código expõe métodos `PublishAsync` e `ConsumeAsync` e mantém wrappers síncronos compatíveis.
- As mensagens usam modelos em `Models/SeasonalFruitMessage.cs` e `Models/UserRegistrationMessage.cs` e incluem campos de data/hora (RequestDateTime e RegistrationDateTime) preenchidos em `Senders` com `DateTime.Now`.

---

## Problemas comuns e troubleshooting

- Erro ao executar `docker` no Windows (ex: pipe error):

  Mensagem típica: "open //./pipe/dockerDesktopLinuxEngine: The system cannot find the file specified"

  Causas comuns:
  - Docker Desktop não está em execução.
  - O backend WSL2/Hyper-V falhou.

  Passos para corrigir:

  1. Abra o Docker Desktop pelo menu Iniciar e espere o ícone da baleia indicar que está rodando.
  2. Rode no PowerShell:

     ```powershell
     docker version
     docker info
     ```

     - Se `docker info` ainda falhar com pipe error, reinicie o Docker Desktop (troubleshoot -> restart) e, se necessário, reinicie o Windows.
  3. Se usar WSL2, verifique com:

     ```powershell
     wsl -l -v
     ```

     e confirme que as distribuições `docker-desktop` e `docker-desktop-data` estão presentes e em `Running`.

- Erro ao adicionar arquivos no Git por causa de `.vs` (Permission denied)

  Se você recebeu algo como:

  ```text
  error: open(".vs/.../.vsidx"): Permission denied
  fatal: adding files failed
  ```

  Solução recomendada:

  1. Feche o Visual Studio/VSCode (libera handles no Windows).
  2. Adicione `.vs/` ao `.gitignore` (já incluído neste repositório).
  3. Se a pasta `.vs` já estiver sendo rastreada, remova-a do índice do Git (não do disco):

     ```powershell
     git rm --cached -r .vs
     git add .gitignore
     git add .
     git commit -m "Ignore Visual Studio .vs folder and remove from index"
     ```

  4. Se o `git rm --cached` falhar por causa de permissão, rode o PowerShell como Administrador ou reinicie o Windows para liberar locks.

---

## Próximas melhorias (opcionais)

- Adicionar volumes persistentes no `docker-compose.yml` para proteger dados do broker entre reinícios.
- Melhorar tolerância a falhas/reconexão no consumidor (circuit-breakers/retries).
- Corrigir os avisos de nulabilidade (marcar propriedades opcionais como `string?` ou inicializá-las) para eliminar warnings do compilador.

---

Se quiser, eu aplico a atualização do `docker-compose.yml` para usar `rabbitmq:3.11-management` e adicionar volume persistente — quer que eu faça isso agora?

   