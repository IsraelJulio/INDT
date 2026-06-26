FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY ContratacaoService.sln .
COPY src/ContratacaoService.Domain/ContratacaoService.Domain.csproj src/ContratacaoService.Domain/
COPY src/ContratacaoService.Application/ContratacaoService.Application.csproj src/ContratacaoService.Application/
COPY src/ContratacaoService.Infrastructure/ContratacaoService.Infrastructure.csproj src/ContratacaoService.Infrastructure/
COPY src/ContratacaoService.API/ContratacaoService.API.csproj src/ContratacaoService.API/
RUN dotnet restore src/ContratacaoService.API/ContratacaoService.API.csproj

COPY . .
RUN dotnet publish src/ContratacaoService.API/ContratacaoService.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ContratacaoService.API.dll"]
