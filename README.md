# Plataforma de Seguros — Teste Técnico INDT

Sistema de gerenciamento de propostas de seguro com dois microserviços em **Arquitetura Hexagonal**.

## Arquitetura

```text
┌─────────────────────────────────────────────────────────────┐
│                        Cliente HTTP                         │
└──────────────┬──────────────────────────┬───────────────────┘
               │                          │
               ▼                          ▼
┌──────────────────────┐    ┌──────────────────────────┐
│   PropostaService    │◄───│   ContratacaoService     │
│   :5001              │    │   :5002                  │
│                      │    │                          │
│  ┌─────────────┐     │    │  ┌─────────────┐        │
│  │ API Layer   │     │    │  │ API Layer   │        │
│  │ (Controllers│     │    │  │ (Controllers│        │
│  └──────┬──────┘     │    │  └──────┬──────┘        │
│         │            │    │         │               │
│  ┌──────▼──────┐     │    │  ┌──────▼──────┐       │
│  │ Application │     │    │  │ Application │       │
│  │ (Use Cases) │     │    │  │ Use Cases)  │       │
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
│  │  Repository)│     │    │  │  HttpClient)│       │
│  └─────────────┘     │    │  └─────────────┘       │
└──────────────────────┘    └──────────────────────────┘
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
- **Infrastructure**: Implementação das portas (EF Core, Repository, HttpClient).
- **API**: Controllers que expõem as portas primárias (Driving Adapters).

---

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL 16
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

- PostgreSQL na porta `5432`
- PropostaService em `http://localhost:5001`
- ContratacaoService em `http://localhost:5002`

As migrations são aplicadas automaticamente na inicialização, caso a aplicação esteja configurada para isso.

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

Com o PostgreSQL já rodando via Docker ou Podman, suba os dois serviços em terminais separados.

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
3. POST /contratacoes           → contrata a proposta aprovada
```

Regra principal:

```text
Apenas propostas com status Aprovada podem ser contratadas.
```

---

## Troubleshooting

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
  - [ ] Contratar proposta aprovada

---

## Stack

- **Runtime**: .NET 10 / ASP.NET Core
- **ORM**: Entity Framework Core 10 + Npgsql
- **Banco**: PostgreSQL 16
- **Testes**: xUnit + Moq
- **Containers**: Docker, Docker Compose e Podman
