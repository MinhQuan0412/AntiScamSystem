FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/AntiScam.Domain/AntiScam.Domain.csproj", "AntiScam.Domain/"]
COPY ["src/AntiScam.Infrastructure/AntiScam.Infrastructure.csproj", "AntiScam.Infrastructure/"]
COPY ["src/AntiScam.Application/AntiScam.Application.csproj", "AntiScam.Application/"]
COPY ["src/AntiScam.API/AntiScam.API.csproj", "AntiScam.API/"]

RUN dotnet restore "AntiScam.API/AntiScam.API.csproj"

COPY src/ .
WORKDIR "/src/AntiScam.API"
RUN dotnet build "AntiScam.API.csproj" -c Release -o /app/build
RUN dotnet publish "AntiScam.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AntiScam.API.dll"]
