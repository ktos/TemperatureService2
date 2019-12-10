FROM microsoft/dotnet:3.0-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:3.0-sdk AS build
WORKDIR /src
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