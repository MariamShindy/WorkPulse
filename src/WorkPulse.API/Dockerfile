FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/WorkPulse.API/WorkPulse.API.csproj", "src/WorkPulse.API/"]
COPY ["src/WorkPulse.Application/WorkPulse.Application.csproj", "src/WorkPulse.Application/"]
COPY ["src/WorkPulse.Infrastructure/WorkPulse.Infrastructure.csproj", "src/WorkPulse.Infrastructure/"]
COPY ["src/WorkPulse.Domain/WorkPulse.Domain.csproj", "src/WorkPulse.Domain/"]
RUN dotnet restore "src/WorkPulse.API/WorkPulse.API.csproj"
COPY . .
WORKDIR "/src/src/WorkPulse.API"
RUN dotnet build "WorkPulse.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WorkPulse.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WorkPulse.API.dll"]
