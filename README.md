# FrutasEpoca RabbitMQ Demo

## Visão Geral

Este projeto demonstra o uso de RabbitMQ com .NET 8 para troca de mensagens entre Senders, um serviço de validação e Receivers, usando tópicos de frutas de época e registro de usuários.

## Estrutura

- **Sender 1**: Envia informações de frutas de época para validação.
- **Sender 2**: Envia informações de registro de usuário para validação.
- **Validation**: Valida as mensagens e encaminha para os Receivers.
- **Receiver 1**: Recebe frutas de época validadas.
- **Receiver 2**: Recebe registros de usuário validados.

## Exchanges, Queues e Routing Keys

- **fruits_exchange**
  - Routing Key: `fruits.validation` → Queue: `fruits_validation_queue`
  - Routing Key: `fruits.receiver1` → Queue: `fruits_receiver1_queue`
- **users_exchange**
  - Routing Key: `users.validation` → Queue: `users_validation_queue`
  - Routing Key: `users.receiver2` → Queue: `users_receiver2_queue`

## Validação

- **Frutas de Época**: Verifica se nome e descrição não estão vazios.
- **Usuário**: Verifica se nome, endereço, RG e CPF não estão vazios.

## Como Executar

1. **Suba o RabbitMQ com Docker:**
   