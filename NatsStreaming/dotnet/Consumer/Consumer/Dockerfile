FROM mcr.microsoft.com/dotnet/core/runtime:3.0.0-buster-slim-arm32v7 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Consumer/Consumer.csproj", "Consumer/"]
RUN dotnet restore -r linux-arm "Consumer/Consumer.csproj"
COPY . .
WORKDIR "/src/Consumer"
RUN dotnet build "Consumer.csproj" -r linux-arm -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Consumer.csproj" -r linux-arm -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Consumer.dll"]