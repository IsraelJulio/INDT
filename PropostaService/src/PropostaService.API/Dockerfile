FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY PropostaService.sln .
COPY src/PropostaService.Domain/PropostaService.Domain.csproj src/PropostaService.Domain/
COPY src/PropostaService.Application/PropostaService.Application.csproj src/PropostaService.Application/
COPY src/PropostaService.Infrastructure/PropostaService.Infrastructure.csproj src/PropostaService.Infrastructure/
COPY src/PropostaService.API/PropostaService.API.csproj src/PropostaService.API/
RUN dotnet restore src/PropostaService.API/PropostaService.API.csproj

COPY . .
RUN dotnet publish src/PropostaService.API/PropostaService.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PropostaService.API.dll"]
