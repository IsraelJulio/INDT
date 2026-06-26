# Plataforma de Seguros — Teste Técnico INDT

Sistema de gerenciamento de propostas de seguro com dois microserviços em **Arquitetura Hexagonal** e comunicação assíncrona via **Kafka**.

## Arquitetura

```text
┌─────────────────────────────────────────────────────────────┐
│                        Cliente HTTP                         │
└──────────────┬──────────────────────────┬───────────────────┘
               │                          │
               ▼                          ▼
┌──────────────────────┐    ┌──────────────────────────┐
│   PropostaService    │    │   ContratacaoService     │
│   :5001              │    │   :5002                  │
│                      │    │                          │
│  ┌─────────────┐     │    │  ┌─────────────┐        │
│  │ API Layer   │     │    │  │ API Layer   │        │
│  │ (Controllers│     │    │  │ (Controllers│        │
│  └──────┬──────┘     │    │  └──────┬──────┘        │
│         │            │    │         │               │
│  ┌──────▼──────┐     │    │  ┌──────▼──────┐       │
│  │ Application │     │    │  │ Application │       │
│  │ (Use Cases) │     │    │  │ (Use Cases) │       │
│  └──────┬──────┘     │    │  └──────┬──────┘       │
│         │            │    │         │               │
│  ┌──────▼──────┐     │    │  ┌──────▼──────┐       │
│  │   Domain    │     │    │  │   Domain    │       │
│  │ (Entities,  │     │    │  │ (Entities,  │       │
│  │  Ports)     │     │    │  │  Ports)     │       │
│  └──────┬──────┘     │    │  └──────┬──────┘       │
│         │            │    │         │               │
│  ┌──────▼──────┐     │    │  ┌──────▼──────┐       │
│  │ Infra       │     │    │  │ Infra       │       │
│  │ (EF Core,   │     │    │  │ (EF Core,   │       │
│  │  Kafka      │     │    │  │  Kafka      │       │
│  │  Publisher) │     │    │  │  Consumer)  │       │
│  └─────────────┘     │    │  └─────────────┘       │
└──────────┬───────────┘    └──────────┬───────────────┘
           │    publica evento          │ consome evento
           │                           │
           └──────────┬────────────────┘
                      ▼
          ┌───────────────────────┐
          │   Kafka :9092         │
          │ topic:                │
          │ proposta-status-      │
          │ atualizada            │
          └───────────────────────┘
               │                          │
               └─────────┬────────────────┘
                         ▼
               ┌──────────────────┐
               │   PostgreSQL     │
               │ PropostaDb       │
               │ ContratacaoDb    │
               └──────────────────┘
```

### Camadas (Hexagonal)

- **Domain**: Entidades, Enums, Portas (interfaces) — sem dependências externas.
- **Application**: Use Cases que orquestram a lógica de negócio.
- **Infrastructure**: Implementação das portas (EF Core, Repository, Kafka Publisher/Consumer).
- **API**: Controllers que expõem as portas primárias (Driving Adapters).

---

## Como o Kafka funciona aqui

Kafka é um sistema de mensagens: um serviço publica uma mensagem e outro serviço lê essa mensagem de forma independente, sem precisar se comunicar diretamente.

Neste projeto o fluxo é:

```text
1. PropostaService aprova uma proposta
2. PropostaService publica uma mensagem no Kafka com o novo status
3. ContratacaoService está rodando em background escutando essas mensagens
4. Quando recebe a mensagem, salva o status no próprio banco de dados (cache local)
5. Quando chega uma requisição de contratação, consulta esse cache local
```

Isso significa que os dois serviços não precisam se chamar diretamente — o Kafka faz a ponte entre eles.

---

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL 16
- Kafka (incluído no `docker-compose.yml`)
- Uma das opções abaixo para container local:
  - [Docker + Docker Compose](https://www.docker.com/), se disponível
  - [Podman](https://podman.io/), alternativa compatível para ambientes onde Docker não é permitido

> Em ambientes corporativos, Docker pode ser bloqueado. Nesse caso, use a seção **Executar localmente com Podman**.

---

## Executar com Docker Compose

Quando Docker estiver disponível:

```bash
docker compose up --build
```

Isso sobe:

- Kafka na porta `9092`
- PostgreSQL na porta `5432`
- PropostaService em `http://localhost:5001`
- ContratacaoService em `http://localhost:5002`

As migrations são aplicadas automaticamente na inicialização. O ContratacaoService aguarda o Kafka estar pronto antes de iniciar.

---

## Executar localmente com Podman

Use esta opção quando Docker não estiver disponível, mas Podman estiver liberado.

### 1. Iniciar a máquina do Podman

No PowerShell:

```powershell
podman machine start
```

Valide se o Podman está funcionando:

```powershell
podman info
```

### 2. Baixar a imagem do PostgreSQL

```powershell
podman pull docker.io/library/postgres:16-alpine
```

Se estiver em rede corporativa e receber erro de certificado como:

```text
tls: failed to verify certificate: x509: certificate signed by unknown authority
```

para teste local, é possível baixar ignorando a validação TLS:

```powershell
podman pull --tls-verify=false docker.io/library/postgres:16-alpine
```

> Observação: `--tls-verify=false` deve ser usado apenas como workaround local. O ideal em ambiente corporativo é instalar o certificado raiz da empresa dentro da máquina Linux usada pelo Podman.

### 3. Subir o PostgreSQL

```powershell
podman run `
  --pull=never `
  --name postgres-dev `
  -e POSTGRES_PASSWORD=postgres `
  -e POSTGRES_USER=postgres `
  -e POSTGRES_DB=insurance_platform `
  -p 5432:5432 `
  -d docker.io/library/postgres:16-alpine
```

Confirme se o container está rodando:

```powershell
podman ps
```

Resultado esperado:

```text
NAMES          STATUS        PORTS
postgres-dev   Up            0.0.0.0:5432->5432/tcp
```

### 4. Comandos úteis do Podman

Ver logs do banco:

```powershell
podman logs postgres-dev
```

Parar o banco:

```powershell
podman stop postgres-dev
```

Iniciar novamente:

```powershell
podman start postgres-dev
```

Remover o container:

```powershell
podman rm -f postgres-dev
```

---

## Connection string local

Para rodar a aplicação localmente usando o PostgreSQL exposto na porta `5432`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=insurance_platform;Username=postgres;Password=postgres"
  }
}
```

Se `localhost` não funcionar no Windows, tente:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=127.0.0.1;Port=5432;Database=insurance_platform;Username=postgres;Password=postgres"
  }
}
```

---

## Aplicar migrations

Se as migrations não forem aplicadas automaticamente ao iniciar a aplicação, rode manualmente.

Na raiz do projeto:

```bash
dotnet ef database update
```

Caso a solução tenha múltiplos projetos e o comando acima não encontre o `DbContext`, informe o projeto de Infrastructure e o projeto de Startup.

Exemplo para o PropostaService:

```bash
dotnet ef database update --project PropostaService/src/PropostaService.Infrastructure --startup-project PropostaService/src/PropostaService.API
```

Exemplo para o ContratacaoService:

```bash
dotnet ef database update --project ContratacaoService/src/ContratacaoService.Infrastructure --startup-project ContratacaoService/src/ContratacaoService.API
```

Se o comando `dotnet ef` não estiver instalado:

```bash
dotnet tool install --global dotnet-ef
```

Depois feche e abra o terminal novamente.

---

## Executar localmente sem Docker Compose

Com o PostgreSQL e o Kafka já rodando, suba os dois serviços em terminais separados.

> Para Kafka local sem Docker Compose, a forma mais simples é usar o [Kafka via download oficial](https://kafka.apache.org/downloads) ou manter o Kafka no Docker Compose e rodar apenas os serviços .NET no terminal.

### Terminal 1 — PropostaService

```bash
cd PropostaService
dotnet run --project src/PropostaService.API --urls "http://localhost:5001"
```

API disponível em:

```text
http://localhost:5001
```

### Terminal 2 — ContratacaoService

```bash
cd ContratacaoService
dotnet run --project src/ContratacaoService.API --urls "http://localhost:5002"
```

API disponível em:

```text
http://localhost:5002
```

---

## Testes automatizados

```bash
# PropostaService
cd PropostaService
dotnet test

# ContratacaoService
cd ../ContratacaoService
dotnet test
```

Ou, se existir arquivo `.sln` na raiz:

```bash
dotnet test
```

---

## Testar a API

É possível testar de três formas:

1. Swagger, se habilitado
2. Postman/Insomnia
3. Terminal/PowerShell

### Swagger

Com os serviços rodando, acesse:

```text
http://localhost:5001/swagger
http://localhost:5002/swagger
```

Se a página abrir, use o botão **Try it out** para executar os endpoints.

Se retornar `404`, o Swagger provavelmente não está habilitado no projeto. Nesse caso, use Postman/Insomnia ou PowerShell.

### Postman ou Insomnia

Crie as requisições abaixo:

#### Criar proposta

```http
POST http://localhost:5001/propostas
Content-Type: application/json
```

Body:

```json
{
  "nomeProponente": "João Silva",
  "cpf": "123.456.789-00",
  "valorCoberto": 50000.0
}
```

#### Aprovar proposta

```http
PATCH http://localhost:5001/propostas/{id}/status
Content-Type: application/json
```

Body:

```json
{
  "status": "Aprovada"
}
```

#### Contratar proposta

```http
POST http://localhost:5002/contratacoes
Content-Type: application/json
```

Body:

```json
{
  "propostaId": "guid-da-proposta"
}
```

### PowerShell

#### Criar proposta

```powershell
$proposta = Invoke-RestMethod `
  -Method Post `
  -Uri "http://localhost:5001/propostas" `
  -ContentType "application/json" `
  -Body '{
    "nomeProponente": "João Silva",
    "cpf": "123.456.789-00",
    "valorCoberto": 50000.00
  }'

$proposta
```

Salvar o ID retornado:

```powershell
$propostaId = $proposta.id
$propostaId
```

#### Aprovar proposta

```powershell
Invoke-RestMethod `
  -Method Patch `
  -Uri "http://localhost:5001/propostas/$propostaId/status" `
  -ContentType "application/json" `
  -Body '{ "status": "Aprovada" }'
```

#### Contratar proposta

```powershell
Invoke-RestMethod `
  -Method Post `
  -Uri "http://localhost:5002/contratacoes" `
  -ContentType "application/json" `
  -Body "{ `"propostaId`": `"$propostaId`" }"
```

---

## API Reference

### PropostaService — `http://localhost:5001`

| Método  | Endpoint                 | Descrição             |
| ------- | ------------------------ | --------------------- |
| `POST`  | `/propostas`             | Criar proposta        |
| `GET`   | `/propostas`             | Listar propostas      |
| `GET`   | `/propostas/{id}`        | Obter proposta por ID |
| `PATCH` | `/propostas/{id}/status` | Atualizar status      |

### POST `/propostas`

```json
{
  "nomeProponente": "João Silva",
  "cpf": "123.456.789-00",
  "valorCoberto": 50000.0
}
```

### PATCH `/propostas/{id}/status`

```json
{
  "status": "Aprovada"
}
```

Status válidos:

```text
EmAnalise
Aprovada
Rejeitada
```

---

### ContratacaoService — `http://localhost:5002`

| Método | Endpoint             | Descrição                   |
| ------ | -------------------- | --------------------------- |
| `POST` | `/contratacoes`      | Contratar proposta aprovada |
| `GET`  | `/contratacoes/{id}` | Obter contratação por ID    |

### POST `/contratacoes`

```json
{
  "propostaId": "guid-da-proposta"
}
```

---

## Fluxo de uso

```text
1. POST /propostas              → cria proposta com status EmAnalise
2. PATCH /propostas/{id}/status → aprova a proposta
                                  (PropostaService publica evento no Kafka)
3. aguardar ~1 segundo          → ContratacaoService lê o evento e salva o status
4. POST /contratacoes           → contrata a proposta aprovada
```

Regras principais:

```text
- Apenas propostas com status Aprovada podem ser contratadas.
- O ContratacaoService não consulta o PropostaService diretamente.
  Ele usa o status que recebeu via Kafka e guardou no próprio banco.
- Se tentar contratar imediatamente após aprovar (sem aguardar),
  pode receber "Proposta não encontrada ou ainda não processada"
  porque o evento do Kafka ainda não chegou.
```

---

## Troubleshooting

### ContratacaoService diz "Proposta não encontrada ou ainda não processada"

Isso acontece quando a contratação é feita antes do evento Kafka chegar.

O fluxo correto é:
1. Aprovar a proposta no PropostaService
2. Aguardar cerca de 1 segundo
3. Tentar contratar

Se o erro persistir mesmo após aguardar, verifique se o Kafka está rodando:

```powershell
docker compose ps
```

O serviço `kafka` deve aparecer com status `healthy`.

---

### ContratacaoService não conecta no Kafka

Erro nos logs:
```text
Broker: Unknown topic or partition
```
ou
```text
Failed to resolve 'kafka:9092'
```

Isso ocorre se o ContratacaoService subiu antes do Kafka estar pronto.

Solução:

```bash
docker compose restart contratacao-service
```

---

### `docker` não é reconhecido

Erro:

```text
docker : The term 'docker' is not recognized
```

Solução:

Use Podman no lugar de Docker:

```powershell
podman run ...
```

---

### `podman` não é reconhecido

Erro:

```text
podman : The term 'podman' is not recognized
```

Solução:

Instale o Podman ou solicite ao time de TI a instalação/liberação dos itens abaixo:

```text
Podman CLI for Windows
WSL2 habilitado
Podman machine configurado
```

---

### `podman-machine-default: VM already exists`

Esse erro indica que a máquina do Podman já foi criada.

Use:

```powershell
podman machine start
```

Se a máquina não iniciar corretamente, verifique o status:

```powershell
podman machine list
wsl -l -v
```

---

### `machine did not transition into running state`

Tente reiniciar o WSL:

```powershell
wsl --shutdown
podman machine start
```

Se continuar falhando e não houver dados importantes na VM, recrie a máquina:

```powershell
podman machine rm -f podman-machine-default
podman machine init
podman machine start
```

---

### Erro de certificado ao baixar imagem

Erro:

```text
tls: failed to verify certificate: x509: certificate signed by unknown authority
```

Workaround local:

```powershell
podman pull --tls-verify=false docker.io/library/postgres:16-alpine
```

Solução recomendada:

Solicite ao time de TI o certificado raiz da empresa em formato `.cer` ou `.crt` e instale-o na máquina Linux usada pelo Podman.

---

### Container com nome já existe

Erro comum:

```text
the container name "postgres-dev" is already in use
```

Solução:

```powershell
podman rm -f postgres-dev
```

Depois suba novamente o PostgreSQL.

---

### Porta 5432 já está em uso

Se outro PostgreSQL estiver usando a porta `5432`, pare o serviço existente ou altere a porta externa:

```powershell
podman run `
  --name postgres-dev `
  -e POSTGRES_PASSWORD=postgres `
  -e POSTGRES_USER=postgres `
  -e POSTGRES_DB=insurance_platform `
  -p 5433:5432 `
  -d docker.io/library/postgres:16-alpine
```

Nesse caso, ajuste a connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=insurance_platform;Username=postgres;Password=postgres"
  }
}
```

---

## Checklist para validar a aplicação

- [ ] Kafka rodando na porta `9092`.
- [ ] PostgreSQL rodando localmente.
- [ ] `dotnet restore` executado com sucesso.
- [ ] `dotnet build` executado com sucesso.
- [ ] Migrations aplicadas no banco.
- [ ] Testes automatizados passando.
- [ ] PropostaService rodando em `http://localhost:5001`.
- [ ] ContratacaoService rodando em `http://localhost:5002`.
- [ ] Fluxo manual validado:
  - [ ] Criar proposta
  - [ ] Aprovar proposta
  - [ ] Aguardar ~1 segundo (propagação do Kafka)
  - [ ] Contratar proposta aprovada

---

## Stack

- **Runtime**: .NET 10 / ASP.NET Core
- **ORM**: Entity Framework Core 10 + Npgsql
- **Banco**: PostgreSQL 16
- **Mensageria**: Apache Kafka 3.7 (Confluent.Kafka)
- **Testes**: xUnit + Moq
- **Containers**: Docker, Docker Compose e Podman
