FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CRN.API/CRN.API.csproj", "CRN.API/"]
COPY ["CRN.Application/CRN.Application.csproj", "CRN.Application/"]
COPY ["CRN.Domain/CRN.Domain.csproj", "CRN.Domain/"]
COPY ["CRN.Infrastructure/CRN.Infrastructure.csproj", "CRN.Infrastructure/"]

RUN dotnet restore "CRN.API/CRN.API.csproj"

COPY . .

WORKDIR "/src/CRN.API"
RUN dotnet build "CRN.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CRN.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "CRN.API.dll"]