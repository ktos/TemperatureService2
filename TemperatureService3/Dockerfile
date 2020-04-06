FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["TemperatureService3.PublicDto/TemperatureService3.PublicDto.csproj", "TemperatureService3.PublicDto/"]
COPY ["TemperatureService3/TemperatureService3.csproj", "TemperatureService3/"]
RUN dotnet restore "TemperatureService3/TemperatureService3.csproj"
COPY . .
WORKDIR "/src/TemperatureService3"
RUN dotnet build "TemperatureService3.csproj" -c Release -o /app

FROM build AS publish
ARG INFOVER="Pre-Release"
RUN dotnet publish "TemperatureService3.csproj" -c Release -o /app /p:InformationalVersion=$INFOVER

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "TemperatureService3.dll"]

HEALTHCHECK CMD curl --fail http://localhost/health || exit